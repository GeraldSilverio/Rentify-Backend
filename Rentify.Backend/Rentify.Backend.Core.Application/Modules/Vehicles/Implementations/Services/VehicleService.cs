using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.RentCars.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.BlockVehicleAvailability;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.ChangeVehicleStatus;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.CreateVehicle;
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

        if (!await _vehicleRepository.ModelExistsAsync(command.ModelId, cancellationToken))
            throw new ApiException("Vehicle model not found.", StatusCodes.Status404NotFound);

        if (await _vehicleRepository.PlateNumberExistsAsync(command.PlateNumber, cancellationToken))
            throw new ApiException($"Vehicle with plate number '{command.PlateNumber}' already exists.", StatusCodes.Status400BadRequest);

        if (await _vehicleRepository.VinExistsAsync(command.Vin, cancellationToken))
            throw new ApiException($"Vehicle with VIN '{command.Vin}' already exists.", StatusCodes.Status400BadRequest);

        StoredImageResult storedImage;

        try
        {
            storedImage = await _imageStorageService.UploadVehicleImageAsync(command.Image, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new ApiException($"Vehicle image upload failed: {ex.Message}", StatusCodes.Status502BadGateway);
        }

        Vehicle vehicle = Vehicle.Create(
            command.RentCarId,
            command.ModelId,
            command.Year,
            command.PlateNumber,
            command.Vin,
            command.Color,
            command.DailyRate,
            storedImage.Url,
            storedImage.PublicId,
            command.CreatedBy);

        try
        {
            await _vehicleRepository.AddAsync(vehicle, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            await _imageStorageService.DeleteAsync(storedImage.PublicId, cancellationToken);
            throw;
        }

        return vehicle.Id;
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
