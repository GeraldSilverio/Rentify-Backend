using MediatR;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Commands.CreateReservation;

public sealed record CreateReservationCommand(
    Guid TenantId,
    Guid CustomerId,
    IReadOnlyCollection<Guid> VehicleIds,
    DateOnly StartDate,
    DateOnly EndDate,
    string CreatedBy) : IRequest<ResultReponse<Guid>>;
