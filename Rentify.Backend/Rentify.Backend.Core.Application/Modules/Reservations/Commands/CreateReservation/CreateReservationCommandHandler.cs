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
using Rentify.Backend.Core.Domain.Entities.Reservations;
using Rentify.Backend.Core.Domain.Entities.Vehicles;
using System.Net;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Commands.CreateReservation;

public sealed class CreateReservationCommandHandler
    : IRequestHandler<CreateReservationCommand, ResultReponse<ReservationResponse>>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IReservationVehicleResolver _vehicleResolver;
    private readonly IReservationLocationResolver _locationResolver;
    private readonly IReservationCodeGenerator _reservationCodeGenerator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomerRepository _customerRepository;

    public CreateReservationCommandHandler(
        IReservationRepository reservationRepository,
        IReservationVehicleResolver vehicleResolver,
        IReservationLocationResolver locationResolver,
        IReservationCodeGenerator reservationCodeGenerator,
        IUnitOfWork unitOfWork,
        ICustomerRepository customerRepository)
    {
        _reservationRepository = reservationRepository;
        _vehicleResolver = vehicleResolver;
        _locationResolver = locationResolver;
        _reservationCodeGenerator = reservationCodeGenerator;
        _unitOfWork = unitOfWork;
        _customerRepository = customerRepository;
    }

    public async Task<ResultReponse<ReservationResponse>> Handle(
        CreateReservationCommand request,
        CancellationToken cancellationToken)
    {
        bool customerExists = await _customerRepository.ExistCustomerByIdAsync(
            request.TenantId,
            request.CustomerId,
            cancellationToken);

        if (!customerExists) throw new ApiException("Customer not found.", HttpStatusCode.NotFound);

        Vehicle vehicle =
            await _vehicleResolver.GetReservableVehicleAsync(
                request.TenantId,
                request.VehicleId,
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

        string code =
            await _reservationCodeGenerator.GenerateAsync(
                request.TenantId,
                cancellationToken);

        Reservation reservation = Reservation.Create(
            request.TenantId,
            code,
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
            request.Channel,
            request.Notes,
            request.CreatedBy);

        await _reservationRepository.AddAsync(reservation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        ReservationResponse response = reservation.ToResponse();

        return ResultReponse<ReservationResponse>.Success(response);
    }
}
