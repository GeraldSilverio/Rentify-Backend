using MediatR; using Rentify.Backend.Core.Application.Modules.Reservations.Dtos; using Rentify.Backend.Core.Application.Modules.Shared.Response;
namespace Rentify.Backend.Core.Application.Modules.Reservations.Commands;
public sealed record CancelReservationCommand(Guid TenantId, Guid ReservationId, string Reason, string CancelledBy) : IRequest<ResultReponse<ReservationResponse>>;
