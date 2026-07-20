using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Queries.GetReservationById;

public sealed record GetReservationByIdQuery(
    Guid TenantId,
    Guid ReservationId) : IRequest<ResultReponse<ReservationDetailsResponse>>;
