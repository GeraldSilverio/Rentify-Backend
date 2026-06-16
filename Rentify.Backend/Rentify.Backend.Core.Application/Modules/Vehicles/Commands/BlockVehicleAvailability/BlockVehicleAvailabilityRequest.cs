namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.BlockVehicleAvailability;

public sealed record BlockVehicleAvailabilityRequest(
    DateOnly StartDate,
    DateOnly EndDate,
    string? Reason,
    string CreatedBy);
