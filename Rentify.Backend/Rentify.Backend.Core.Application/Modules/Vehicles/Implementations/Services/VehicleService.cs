using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Storage;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.BlockVehicleAvailability;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.ChangeVehicleStatus;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.CreateVehicle;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.DeleteVehicle;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.SetPrimaryVehicleImage;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UpdateVehicle;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UploadVehicleImage;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Services;
using Rentify.Backend.Core.Domain.Entities.Vehicles;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Implementations.Services;

public sealed class VehicleService : IVehicleService
{
    private const string VehicleImagesFolder = "vehicles";
    private const int MaxImagesPerVehicle = 5;

    private readonly IVehicleRepository _vehicleRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;

    public VehicleService(
        IVehicleRepository vehicleRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork)
    {
        _vehicleRepository = vehicleRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> CreateAsync(CreateVehicleCommand command, CancellationToken cancellationToken = default)
    {
        if (await _vehicleRepository.PlateNumberExistsAsync(command.TenantId, command.PlateNumber, cancellationToken: cancellationToken))
            throw new ApiException($"Vehicle with plate number '{command.PlateNumber}' already exists for this tenant.", StatusCodes.Status400BadRequest);

        if (await _vehicleRepository.VinExistsAsync(command.TenantId, command.Vin, cancellationToken: cancellationToken))
            throw new ApiException($"Vehicle with VI" +
                $"N '{command.Vin}' already exists for this tenant.", StatusCodes.Status400BadRequest);

        Vehicle vehicle = Vehicle.Create(
            command.TenantId,
            command.VehicleBrandId,
            command.VehicleModelId,
            command.VehicleTypeId,
            command.Year,
            command.PlateNumber,
            command.Vin,
            command.Color,
            command.CurrentMileage,
            command.CreatedBy);

        vehicle.ReplaceRates(
            [(RentalType.Daily, command.DailyRate)],
            command.CreatedBy);

        await _vehicleRepository.AddAsync(vehicle, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return vehicle.Id;
    }

    public async Task UpdateAsync(UpdateVehicleCommand command, CancellationToken cancellationToken = default)
    {
        Vehicle vehicle = await GetVehicleOrThrowAsync(command.TenantId, command.VehicleId, cancellationToken);

        if (await _vehicleRepository.PlateNumberExistsAsync(command.TenantId, command.PlateNumber, command.VehicleId, cancellationToken))
            throw new ApiException($"Vehicle with plate number '{command.PlateNumber}' already exists for this tenant.", StatusCodes.Status400BadRequest);

        if (await _vehicleRepository.VinExistsAsync(command.TenantId, command.Vin, command.VehicleId, cancellationToken))
            throw new ApiException($"Vehicle with VIN '{command.Vin}' already exists for this tenant.", StatusCodes.Status400BadRequest);

        vehicle.Update(
            command.VehicleBrandId,
            command.VehicleModelId,
            command.VehicleTypeId,
            command.Year,
            command.PlateNumber,
            command.Vin,
            command.Color,
            command.CurrentMileage,
            command.ModifiedBy);

        vehicle.ReplaceRates(
            [(RentalType.Daily, command.DailyRate)],
            command.ModifiedBy);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(DeleteVehicleCommand command, CancellationToken cancellationToken = default)
    {
        Vehicle vehicle = await GetVehicleOrThrowAsync(command.TenantId, command.VehicleId, cancellationToken);

        vehicle.Delete(command.ModifiedBy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<VehicleImageResponse>> UploadImagesAsync(
        UploadVehicleImageCommand command,
        CancellationToken cancellationToken = default)
    {
        Vehicle vehicle = await GetVehicleWithImagesOrThrowAsync(command.TenantId, command.VehicleId, cancellationToken);

        int existingImagesCount = vehicle.Images.Count(image => !image.IsDeleted);
        if (existingImagesCount + command.Images.Count > MaxImagesPerVehicle)
        {
            throw new ApiException(
                $"A vehicle can have a maximum of {MaxImagesPerVehicle} images.",
                StatusCodes.Status400BadRequest);
        }

        var uploadedFiles = new List<StoredFileResult>();

        try
        {
            foreach (IFormFile file in command.Images)
            {
                uploadedFiles.Add(await _fileStorageService.UploadAsync(file, VehicleImagesFolder, cancellationToken));
            }
        }
        catch (Exception ex)
        {
            await DeleteUploadedFilesAsync(uploadedFiles);
            throw new ApiException($"Vehicle image upload failed: {ex.Message}", StatusCodes.Status502BadGateway);
        }

        var images = new List<VehicleImage>(uploadedFiles.Count);

        for (int index = 0; index < uploadedFiles.Count; index++)
        {
            StoredFileResult storedFile = uploadedFiles[index];

            // A batch has one primary intent. The first new image receives it;
            // Vehicle.AddImage preserves the invariant for the full collection.
            bool isPrimary = command.IsPrimary && index == 0;
            images.Add(vehicle.AddImage(storedFile.Url, storedFile.PublicId, isPrimary, command.CreatedBy));
        }

        // The aggregate is tracked and owns the relationship; AddRange makes the
        // state of the new children explicit without attaching or updating the vehicle.
        await _vehicleRepository.AddImagesAsync(images, cancellationToken);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (ConcurrencyException)
        {
            await DeleteUploadedFilesAsync(uploadedFiles);
            throw new ApiException(
                "Vehicle was changed or deleted while its images were being uploaded. Please retry.",
                StatusCodes.Status409Conflict);
        }
        catch
        {
            await DeleteUploadedFilesAsync(uploadedFiles);
            throw;
        }

        return images
            .Select(image => new VehicleImageResponse(image.Id, image.Url, image.IsPrimary))
            .ToList();
    }

    public async Task SetPrimaryImageAsync(
        SetPrimaryVehicleImageCommand command,
        CancellationToken cancellationToken = default)
    {
        Vehicle vehicle = await GetVehicleWithImagesOrThrowAsync(command.TenantId, command.VehicleId, cancellationToken);

        try
        {
            vehicle.SetPrimaryImage(command.ImageId, command.ModifiedBy);
        }
        catch (ArgumentException ex)
        {
            throw new ApiException(ex.Message, StatusCodes.Status404NotFound);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ChangeStatusAsync(ChangeVehicleStatusCommand command, CancellationToken cancellationToken = default)
    {
        Vehicle vehicle = await GetVehicleOrThrowAsync(command.TenantId, command.VehicleId, cancellationToken);

        vehicle.ChangeStatus(command.Status, command.ModifiedBy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task BlockAvailabilityAsync(BlockVehicleAvailabilityCommand command, CancellationToken cancellationToken = default)
    {
        Vehicle vehicle = await GetVehicleOrThrowAsync(command.TenantId, command.VehicleId, cancellationToken);

        try
        {
            vehicle.AddUnavailableDate(command.StartDate, command.EndDate, command.Reason, command.CreatedBy);
        }
        catch (ArgumentException ex)
        {
            throw new ApiException(ex.Message, StatusCodes.Status400BadRequest);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<Vehicle> GetVehicleOrThrowAsync(Guid tenantId, Guid vehicleId, CancellationToken cancellationToken)
    {
        return await _vehicleRepository.GetByIdAsync(tenantId, vehicleId, cancellationToken)
               ?? throw new ApiException("Vehicle not found.", StatusCodes.Status404NotFound);
    }

    private async Task<Vehicle> GetVehicleWithImagesOrThrowAsync(Guid tenantId, Guid vehicleId, CancellationToken cancellationToken)
    {
        return await _vehicleRepository.GetByIdWithImagesAsync(tenantId, vehicleId, cancellationToken)
               ?? throw new ApiException("Vehicle not found.", StatusCodes.Status404NotFound);
    }
    private async Task DeleteUploadedFilesAsync(IEnumerable<StoredFileResult> uploadedFiles)
    {
        foreach (StoredFileResult uploadedFile in uploadedFiles)
        {
            try
            {
                await _fileStorageService.DeleteAsync(uploadedFile.PublicId, CancellationToken.None);
            }
            catch
            {
                // Preserve the original failure; cleanup can be retried operationally.
            }
        }
    }
}
