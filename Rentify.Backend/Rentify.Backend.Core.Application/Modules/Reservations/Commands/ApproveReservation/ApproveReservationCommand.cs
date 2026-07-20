using MediatR;
using Rentify.Backend.Core.Application.Modules.Reservations.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Commands;

public sealed record ApproveReservationCommand(
    Guid TenantId,
    Guid ReservationId,
    string ApprovedBy) : IRequest<ResultReponse<ReservationResponse>>;
