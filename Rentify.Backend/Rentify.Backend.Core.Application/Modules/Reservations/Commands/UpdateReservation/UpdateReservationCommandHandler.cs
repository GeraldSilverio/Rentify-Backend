using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Customers.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Reservations.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Reservations.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Reservations.Dtos;
using Rentify.Backend.Core.Application.Modules.Reservations.Mappers;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Domain.Entities.Customers;
using Rentify.Backend.Core.Domain.Entities.Reservations;
using Rentify.Backend.Core.Domain.Entities.Vehicles;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Commands.UpdateReservation;

public sealed class UpdateReservationCommandHandler
    : IRequestHandler<UpdateReservationCommand, ResultReponse<ReservationResponse>>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IReservationVehicleResolver _vehicleResolver;
    private readonly IReservationLocationResolver _locationResolver;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomerRepository _customerRepository;

    public UpdateReservationCommandHandler(
        IReservationRepository reservationRepository,
        IReservationVehicleResolver vehicleResolver,
        IReservationLocationResolver locationResolver,
        IUnitOfWork unitOfWork,
        ICustomerRepository customerRepository)
    {
        _reservationRepository = reservationRepository;
        _vehicleResolver = vehicleResolver;
        _locationResolver = locationResolver;
        _unitOfWork = unitOfWork;
        _customerRepository = customerRepository;
    }

    public async Task<ResultReponse<ReservationResponse>> Handle(
        UpdateReservationCommand request,
        CancellationToken cancellationToken)
    {
        bool existsCustomer =
            await _customerRepository.ExistCustomerByIdAsync(
                request.TenantId,
                request.CustomerId,
                cancellationToken);

        if (!existsCustomer) throw new ApiException(
            "Cliente no encontrado.",
            StatusCodes.Status404NotFound);

        Reservation reservation =
            await _reservationRepository.GetByIdAsync(
                request.TenantId,
                request.ReservationId,
                cancellationToken)
            ?? throw new ApiException(
                "Reserva no encontrada.",
                StatusCodes.Status404NotFound);

        Vehicle vehicle =
            await _vehicleResolver.GetReservableVehicleAsync(
                request.TenantId,
                reservation.VehicleId,
                cancellationToken);

        ResolvedReservationLocation delivery =
            await _locationResolver.ResolveDeliveryAsync(
                request.TenantId,
                request.DeliveryTenantLocationId,
                request.DeliveryLocationName,
                request.DeliveryFee,
                cancellationToken);

        ResolvedReservationLocation returns =
            await _locationResolver.ResolveReturnAsync(
                request.TenantId,
                request.ReturnTenantLocationId,
                request.ReturnLocationName,
                request.ReturnFee,
                cancellationToken);

        VehicleRate rate = _vehicleResolver.GetRateOrThrow(vehicle, request.RentalType);

        int quantity = Reservation.CalculateQuantity(
            request.DeliveryDateTime,
            request.ExpectedReturnDateTime,
            request.RentalType);

        reservation.UpdatePendingReservation(
            request.CustomerId,
            request.VehicleId,
            request.DeliveryDateTime,
            request.ExpectedReturnDateTime,
            request.RentalType,
            quantity,
            rate.Price,
            vehicle.SecurityDepositRequired,
            vehicle.SecurityDepositAmount,
            delivery.TenantLocationId,
            delivery.Name,
            request.DeliveryAddressDetails,
            delivery.Fee,
            returns.TenantLocationId,
            returns.Name,
            request.ReturnAddressDetails,
            returns.Fee,
            request.DiscountAmount,
            reservation.Channel,
            request.Notes,
            request.ModifiedBy);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        ReservationResponse response = reservation.ToResponse();

        return ResultReponse<ReservationResponse>.Success(response);
    }
}
