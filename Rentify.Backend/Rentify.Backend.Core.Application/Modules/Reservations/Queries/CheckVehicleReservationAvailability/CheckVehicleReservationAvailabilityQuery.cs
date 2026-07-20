using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Queries.CheckVehicleReservationAvailability;

public sealed record CheckVehicleReservationAvailabilityQuery(
    Guid TenantId,
    Guid VehicleId,
    DateTime DeliveryDateTime,
    DateTime ExpectedReturnDateTime,
    Guid? ExcludeReservationId) : IRequest<ResultReponse<ReservationAvailabilityResponse>>;
