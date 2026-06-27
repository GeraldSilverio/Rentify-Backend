using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Customers.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Reservations.Commands.CreateReservation;
using Rentify.Backend.Core.Application.Modules.Reservations.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Reservations.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Domain.Entities;
using Rentify.Backend.Core.Domain.Entities.Reservations;
using Rentify.Backend.Core.Domain.Entities.Vehicles;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Implementations.Services;

public sealed class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReservationService(
        IReservationRepository reservationRepository,
        ICustomerRepository customerRepository,
        IVehicleRepository vehicleRepository,
        IUnitOfWork unitOfWork)
    {
        _reservationRepository = reservationRepository;
        _customerRepository = customerRepository;
        _vehicleRepository = vehicleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> CreateAsync(CreateReservationCommand command, CancellationToken cancellationToken = default)
    {
        if (await _customerRepository.GetByIdAsync(command.TenantId, command.CustomerId, cancellationToken) is null)
            throw new ApiException("Customer not found.", StatusCodes.Status404NotFound);

        Guid[] vehicleIds = command.VehicleIds.Distinct().ToArray();
        if (vehicleIds.Length != command.VehicleIds.Count)
            throw new ApiException("Reservation contains duplicated vehicles.", StatusCodes.Status400BadRequest);

        if (await _reservationRepository.HasOverlappingReservationAsync(command.TenantId, vehicleIds, command.StartDate, command.EndDate, cancellationToken))
            throw new ApiException("One or more vehicles are already reserved for this date range.", StatusCodes.Status400BadRequest);

        Reservation reservation = Reservation.Create(
            command.TenantId,
            command.CustomerId,
            command.StartDate,
            command.EndDate,
            command.CreatedBy);

        foreach (Guid vehicleId in vehicleIds)
        {
            Vehicle vehicle = await _vehicleRepository.GetByIdAsync(command.TenantId, vehicleId, cancellationToken)
                              ?? throw new ApiException("Vehicle not found.", StatusCodes.Status404NotFound);

            if (vehicle.Status is VehicleStatus.Maintenance or VehicleStatus.Unavailable)
                throw new ApiException($"Vehicle '{vehicle.PlateNumber}' is not available.", StatusCodes.Status400BadRequest);

            if (!vehicle.IsAvailableFor(command.StartDate, command.EndDate))
                throw new ApiException($"Vehicle '{vehicle.PlateNumber}' is unavailable for this date range.", StatusCodes.Status400BadRequest);

            reservation.AddVehicle(vehicle.Id, vehicle.DailyRate, command.CreatedBy);
            vehicle.ChangeStatus(VehicleStatus.Reserved, command.CreatedBy);
        }

        await _reservationRepository.AddAsync(reservation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return reservation.Id;
    }
}
