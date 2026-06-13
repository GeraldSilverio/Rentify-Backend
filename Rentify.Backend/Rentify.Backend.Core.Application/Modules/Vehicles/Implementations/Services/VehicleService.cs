using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.RentCars.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.BlockVehicleAvailability;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.ChangeVehicleStatus;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.CreateVehicle;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.DeleteVehicleImage;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UploadVehicleImage;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Services;
using Rentify.Backend.Core.Application.Shared.Exceptions;
using Rentify.Backend.Core.Application.Shared.UnitOfWork;
using Rentify.Backend.Core.Domain.Entities;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Implementations.Services;

public sealed class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IRentCarRepository _rentCarRepository;
    private readonly IImageStorageService _imageStorageService;
    private readonly IUnitOfWork _unitOfWork;

    public VehicleService(
        IVehicleRepository vehicleRepository,
        IRentCarRepository rentCarRepository,
        IImageStorageService imageStorageService,
        IUnitOfWork unitOfWork)
    {
        _vehicleRepository = vehicleRepository;
        _rentCarRepository = rentCarRepository;
        _imageStorageService = imageStorageService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> CreateAsync(CreateVehicleCommand command, CancellationToken cancellationToken = default)
    {
        if (await _rentCarRepository.GetByIdAsync(command.RentCarId, cancellationToken) is null)
            throw new ApiException("Rent car profile not found.", StatusCodes.Status404NotFound);

        if (await _vehicleRepository.PlateNumberExistsAsync(command.PlateNumber, cancellationToken))
            throw new ApiException($"Vehicle with plate number '{command.PlateNumber}' already exists.", StatusCodes.Status400BadRequest);

        if (await _vehicleRepository.VinExistsAsync(command.Vin, cancellationToken))
            throw new ApiException($"Vehicle with VIN '{command.Vin}' already exists.", StatusCodes.Status400BadRequest);

        Vehicle vehicle = Vehicle.Create(
            command.RentCarId,
            command.Make,
            command.Model,
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

    public async Task<Guid> UploadImageAsync(UploadVehicleImageCommand command, CancellationToken cancellationToken = default)
    {
        Vehicle vehicle = await GetVehicleOrThrowAsync(command.VehicleId, cancellationToken);
        StoredImageResult storedImage = await _imageStorageService.SaveVehicleImageAsync(
            command.VehicleId,
            command.Image,
            cancellationToken);

        VehicleImage image = VehicleImage.Create(
            vehicle.Id,
            storedImage.Url,
            storedImage.FileName,
            storedImage.ContentType,
            storedImage.SizeInBytes,
            command.IsPrimary || !vehicle.Images.Any(x => !x.IsDeleted),
            command.CreatedBy);

        vehicle.AddImage(image, command.CreatedBy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return image.Id;
    }

    public async Task DeleteImageAsync(DeleteVehicleImageCommand command, CancellationToken cancellationToken = default)
    {
        Vehicle vehicle = await GetVehicleOrThrowAsync(command.VehicleId, cancellationToken);
        VehicleImage image = vehicle.Images.FirstOrDefault(x => x.Id == command.ImageId && !x.IsDeleted)
                             ?? throw new ApiException("Vehicle image not found.", StatusCodes.Status404NotFound);

        vehicle.DeleteImage(command.ImageId, command.ModifiedBy);
        await _imageStorageService.DeleteAsync(image.Url, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ChangeStatusAsync(ChangeVehicleStatusCommand command, CancellationToken cancellationToken = default)
    {
        Vehicle vehicle = await GetVehicleOrThrowAsync(command.VehicleId, cancellationToken);

        vehicle.ChangeStatus(command.Status, command.ModifiedBy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task BlockAvailabilityAsync(BlockVehicleAvailabilityCommand command, CancellationToken cancellationToken = default)
    {
        Vehicle vehicle = await GetVehicleOrThrowAsync(command.VehicleId, cancellationToken);

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

    private async Task<Vehicle> GetVehicleOrThrowAsync(Guid vehicleId, CancellationToken cancellationToken)
    {
        return await _vehicleRepository.GetByIdAsync(vehicleId, cancellationToken)
               ?? throw new ApiException("Vehicle not found.", StatusCodes.Status404NotFound);
    }
}
