using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Rentify.Backend.Core.Application.Modules.Reservations.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Reservations.Dtos;
using Rentify.Backend.Core.Application.Modules.Reservations.Mappers;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Application.Modules.Shared.Context;
using Rentify.Backend.Core.Application.Modules.Shared.Logging;
using Rentify.Backend.Core.Domain.Entities.Reservations;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Commands.RejectReservation;

public sealed class RejectReservationCommandHandler
    : IRequestHandler<RejectReservationCommand, ResultReponse<ReservationResponse>>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentRequestContext _currentRequestContext;
    private readonly ILogger<RejectReservationCommandHandler> _logger;

    public RejectReservationCommandHandler(
        IReservationRepository reservationRepository,
        IUnitOfWork unitOfWork,
        ICurrentRequestContext currentRequestContext,
        ILogger<RejectReservationCommandHandler> logger)
    {
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
        _currentRequestContext = currentRequestContext;
        _logger = logger;
    }

    public async Task<ResultReponse<ReservationResponse>> Handle(
        RejectReservationCommand request,
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

        ReservationStatus previousStatus = reservation.Status;
        reservation.Reject(request.Reason, request.RejectedBy);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Reservation {ReservationId} rejected in Tenant {TenantId} by {RejectedBy} with status {PreviousStatus} to {NewStatus}",
            reservation.Id,
            reservation.TenantId,
            RequestContextLogValues.GetUserId(_currentRequestContext),
            previousStatus,
            reservation.Status);

        ReservationResponse response = reservation.ToResponse();

        return ResultReponse<ReservationResponse>.Success(response);
    }
}
