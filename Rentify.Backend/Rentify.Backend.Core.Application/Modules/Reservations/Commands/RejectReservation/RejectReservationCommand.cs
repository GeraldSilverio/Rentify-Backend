using MediatR;
using Rentify.Backend.Core.Application.Modules.Reservations.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Commands.RejectReservation;

public sealed record RejectReservationCommand(
    Guid TenantId,
    Guid ReservationId,
    string Reason,
    string RejectedBy) : IRequest<ResultReponse<ReservationResponse>>;
