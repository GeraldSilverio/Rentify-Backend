using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.RentCars.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.BlockVehicleAvailability;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.ChangeVehicleStatus;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.CreateVehicle;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.DeleteVehicle;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.SetPrimaryVehicleImage;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UpdateVehicle;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UploadVehicleImage;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Services;
using Rentify.Backend.Core.Application.Shared.Exceptions;
using Rentify.Backend.Core.Application.Shared.Storage;
using Rentify.Backend.Core.Application.Shared.UnitOfWork;
using Rentify.Backend.Core.Domain.Entities;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Implementations.Services;

public sealed class VehicleService : IVehicleService
{
    private const string VehicleImagesFolder = "vehicles";

    private readonly IVehicleRepository _vehicleRepository;
    private readonly IRentCarRepository _rentCarRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;

    public VehicleService(
        IVehicleRepository vehicleRepository,
        IRentCarRepository rentCarRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork)
    {
        _vehicleRepository = vehicleRepository;
        _rentCarRepository = rentCarRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> CreateAsync(CreateVehicleCommand command, CancellationToken cancellationToken = default)
    {
        RentCar rentCar = await GetRentCarOrThrowAsync(command.RentCarId, command.TenantId, cancellationToken);

        if (!await _vehicleRepository.VehicleModelExistsAsync(command.VehicleModelId, cancellationToken))
            throw new ApiException("Vehicle model not found.", StatusCodes.Status404NotFound);

        if (!await _vehicleRepository.VehicleTypeExistsAsync(command.TenantId, command.VehicleTypeId, cancellationToken))
            throw new ApiException("Vehicle type not found.", StatusCodes.Status404NotFound);

        if (await _vehicleRepository.PlateNumberExistsAsync(command.TenantId, command.PlateNumber, cancellationToken: cancellationToken))
            throw new ApiException($"Vehicle with plate number '{command.PlateNumber}' already exists for this tenant.", StatusCodes.Status400BadRequest);

        if (await _vehicleRepository.VinExistsAsync(command.TenantId, command.Vin, cancellationToken: cancellationToken))
            throw new ApiException($"Vehicle with VIN '{command.Vin}' already exists for this tenant.", StatusCodes.Status400BadRequest);

        Vehicle vehicle = Vehicle.Create(
            command.TenantId,
            rentCar.Id,
            command.VehicleModelId,
            command.VehicleTypeId,
            command.Year,
            command.PlateNumber,
            command.Vin,
            command.Color,
            command.DailyRate,
            command.CreatedBy);

        await _vehicleRepository.AddAsync(vehicle, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return vehicle.Id;
    }

    public async Task UpdateAsync(UpdateVehicleCommand command, CancellationToken cancellationToken = default)
    {
        Vehicle vehicle = await GetVehicleOrThrowAsync(command.TenantId, command.VehicleId, cancellationToken);

        if (!await _vehicleRepository.VehicleModelExistsAsync(command.VehicleModelId, cancellationToken))
            throw new ApiException("Vehicle model not found.", StatusCodes.Status404NotFound);

        if (!await _vehicleRepository.VehicleTypeExistsAsync(command.TenantId, command.VehicleTypeId, cancellationToken))
            throw new ApiException("Vehicle type not found.", StatusCodes.Status404NotFound);

        if (await _vehicleRepository.PlateNumberExistsAsync(command.TenantId, command.PlateNumber, command.VehicleId, cancellationToken))
            throw new ApiException($"Vehicle with plate number '{command.PlateNumber}' already exists for this tenant.", StatusCodes.Status400BadRequest);

        if (await _vehicleRepository.VinExistsAsync(command.TenantId, command.Vin, command.VehicleId, cancellationToken))
            throw new ApiException($"Vehicle with VIN '{command.Vin}' already exists for this tenant.", StatusCodes.Status400BadRequest);

        vehicle.Update(
            command.VehicleModelId,
            command.VehicleTypeId,
            command.Year,
            command.PlateNumber,
            command.Vin,
            command.Color,
            command.DailyRate,
            command.ModifiedBy);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(DeleteVehicleCommand command, CancellationToken cancellationToken = default)
    {
        Vehicle vehicle = await GetVehicleOrThrowAsync(command.TenantId, command.VehicleId, cancellationToken);

        vehicle.Delete(command.ModifiedBy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Guid> UploadImageAsync(
        UploadVehicleImageCommand command,
        CancellationToken cancellationToken = default)
    {
        Vehicle vehicle = await GetVehicleWithImagesOrThrowAsync(command.TenantId, command.VehicleId, cancellationToken);

        StoredFileResult storedFile;

        try
        {
            storedFile = await _fileStorageService.UploadAsync(command.Image, VehicleImagesFolder, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new ApiException($"Vehicle image upload failed: {ex.Message}", StatusCodes.Status502BadGateway);
        }

        VehicleImage image = vehicle.AddImage(storedFile.Url, storedFile.PublicId, command.IsPrimary, command.CreatedBy);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            await _fileStorageService.DeleteAsync(storedFile.PublicId, cancellationToken);
            throw;
        }

        return image.Id;
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

    private async Task<RentCar> GetRentCarOrThrowAsync(Guid rentCarId, Guid tenantId, CancellationToken cancellationToken)
    {
        RentCar rentCar = await _rentCarRepository.GetByIdAsync(rentCarId, cancellationToken)
                          ?? throw new ApiException("Rent car profile not found.", StatusCodes.Status404NotFound);

        if (rentCar.TenantId != tenantId)
            throw new ApiException("Rent car profile not found for this tenant.", StatusCodes.Status404NotFound);

        return rentCar;
    }
}
