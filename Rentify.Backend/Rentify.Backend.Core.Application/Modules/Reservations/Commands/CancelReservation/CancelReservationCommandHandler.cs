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

namespace Rentify.Backend.Core.Application.Modules.Reservations.Commands.CancelReservation;

public sealed class CancelReservationCommandHandler
    : IRequestHandler<CancelReservationCommand, ResultReponse<ReservationResponse>>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentRequestContext _currentRequestContext;
    private readonly ILogger<CancelReservationCommandHandler> _logger;

    public CancelReservationCommandHandler(
        IReservationRepository reservationRepository,
        IUnitOfWork unitOfWork,
        ICurrentRequestContext currentRequestContext,
        ILogger<CancelReservationCommandHandler> logger)
    {
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
        _currentRequestContext = currentRequestContext;
        _logger = logger;
    }

    public async Task<ResultReponse<ReservationResponse>> Handle(
        CancelReservationCommand request,
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
        reservation.Cancel(request.Reason, request.CancelledBy);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Reservation {ReservationId} cancelled in Tenant {TenantId} by {CancelledBy} with status {PreviousStatus} to {NewStatus}",
            reservation.Id,
            reservation.TenantId,
            RequestContextLogValues.GetUserId(_currentRequestContext),
            previousStatus,
            reservation.Status);

        ReservationResponse response = reservation.ToResponse();

        return ResultReponse<ReservationResponse>.Success(response);
    }
}
