using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Reservations.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Reservations.Dtos;
using Rentify.Backend.Core.Application.Modules.Reservations.Mappers;
using Rentify.Backend.Core.Application.Modules.Reservations.Services;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Domain.Entities.Reservations;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Commands;

public sealed class ApproveReservationCommandHandler
    : IRequestHandler<ApproveReservationCommand, ResultReponse<ReservationResponse>>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IReservationCustomerValidator _customerValidator;
    private readonly IReservationVehicleResolver _vehicleResolver;
    private readonly IUnitOfWork _unitOfWork;

    public ApproveReservationCommandHandler(
        IReservationRepository reservationRepository,
        IReservationCustomerValidator customerValidator,
        IReservationVehicleResolver vehicleResolver,
        IUnitOfWork unitOfWork)
    {
        _reservationRepository = reservationRepository;
        _customerValidator = customerValidator;
        _vehicleResolver = vehicleResolver;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultReponse<ReservationResponse>> Handle(
        ApproveReservationCommand request,
        CancellationToken cancellationToken)
    {
        Reservation reservation =
            await _reservationRepository.GetByIdAsync(
                request.TenantId,
                request.ReservationId,
                cancellationToken)
            ?? throw new ApiException(
                "Reserva no encontrada.",
                StatusCodes.Status404NotFound);

        await _customerValidator.ValidateAsync(
            request.TenantId,
            reservation.CustomerId,
            cancellationToken);

        await _vehicleResolver.GetReservableVehicleAsync(
            request.TenantId,
            reservation.VehicleId,
            cancellationToken);

        bool hasApprovedOverlap =
            await _reservationRepository.HasApprovedOverlapAsync(
                request.TenantId,
                reservation.VehicleId,
                reservation.DeliveryDateTime,
                reservation.ExpectedReturnDateTime,
                reservation.Id,
                cancellationToken);

        if (hasApprovedOverlap)
        {
            throw new ApiException(
                "El vehículo ya tiene una reserva aprobada para el rango de fechas seleccionado.",
                StatusCodes.Status400BadRequest);
        }

        reservation.Approve(request.ApprovedBy);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        ReservationResponse response = reservation.ToResponse();

        return ResultReponse<ReservationResponse>.Success(response);
    }
}
