using MediatR;
using Rentify.Backend.Core.Application.Modules.Reservations.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Queries;

public sealed record CheckVehicleReservationAvailabilityQuery(
    Guid TenantId,
    Guid VehicleId,
    DateTime DeliveryDateTime,
    DateTime ExpectedReturnDateTime,
    Guid? ExcludeReservationId) : IRequest<ResultReponse<ReservationAvailabilityResponse>>;
