namespace Rentify.Backend.Core.Application.Modules.Reservations.Queries.CheckVehicleReservationAvailability;

public sealed record ReservationAvailabilityResponse(
    bool Available,
    string? Message);
