using MediatR;
using Microsoft.Extensions.Logging;
using Rentify.Backend.Core.Application.Modules.Reservations.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Queries.CheckVehicleReservationAvailability;

public sealed class CheckVehicleReservationAvailabilityQueryHandler
    : IRequestHandler<CheckVehicleReservationAvailabilityQuery, ResultReponse<ReservationAvailabilityResponse>>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly ILogger<CheckVehicleReservationAvailabilityQueryHandler> _logger;

    public CheckVehicleReservationAvailabilityQueryHandler(
        IReservationRepository reservationRepository,
        ILogger<CheckVehicleReservationAvailabilityQueryHandler> logger)
    {
        _reservationRepository = reservationRepository;
        _logger = logger;
    }

    public async Task<ResultReponse<ReservationAvailabilityResponse>> Handle(
        CheckVehicleReservationAvailabilityQuery request,
        CancellationToken cancellationToken)
    {
        bool occupied =
            await _reservationRepository.HasApprovedOverlapAsync(
                request.TenantId,
                request.VehicleId,
                request.DeliveryDateTime,
                request.ExpectedReturnDateTime,
                request.ExcludeReservationId,
                cancellationToken);

        if (occupied)
        {
            _logger.LogWarning(
                "Vehicle availability conflict detected for Vehicle {VehicleId} from {StartDateTime} to {EndDateTime} in Tenant {TenantId}",
                request.VehicleId,
                request.DeliveryDateTime,
                request.ExpectedReturnDateTime,
                request.TenantId);
        }

        ReservationAvailabilityResponse response = new(
            !occupied,
            occupied
                ? "El vehículo ya tiene una reserva aprobada para el rango de fechas seleccionado."
                : null);

        return ResultReponse<ReservationAvailabilityResponse>.Success(response);
    }
}
