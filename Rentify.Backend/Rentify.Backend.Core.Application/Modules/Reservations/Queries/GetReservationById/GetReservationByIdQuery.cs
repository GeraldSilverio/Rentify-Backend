using MediatR;
using Rentify.Backend.Core.Application.Modules.Reservations.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Queries;

public sealed record GetReservationByIdQuery(Guid TenantId, Guid ReservationId) : IRequest<ResultReponse<ReservationDetailsResponse>>;
