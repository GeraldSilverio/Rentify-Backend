using MediatR; using Rentify.Backend.Core.Application.Modules.Shared.Response;
namespace Rentify.Backend.Core.Application.Modules.Reservations.Commands;
public sealed record DeleteReservationCommand(Guid TenantId, Guid ReservationId, string ModifiedBy) : IRequest<ResultReponse<Guid>>;
