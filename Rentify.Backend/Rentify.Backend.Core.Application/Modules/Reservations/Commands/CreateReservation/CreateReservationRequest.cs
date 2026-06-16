namespace Rentify.Backend.Core.Application.Modules.Reservations.Commands.CreateReservation;

public sealed record CreateReservationRequest(
    Guid CustomerId,
    IReadOnlyCollection<Guid> VehicleIds,
    DateOnly StartDate,
    DateOnly EndDate,
    string CreatedBy);
