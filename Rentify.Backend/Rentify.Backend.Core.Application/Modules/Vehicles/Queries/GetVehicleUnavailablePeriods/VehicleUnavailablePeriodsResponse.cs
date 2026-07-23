namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicleUnavailablePeriods;

public sealed record VehicleUnavailablePeriodsResponse(
    Guid VehicleId,
    IReadOnlyCollection<VehicleUnavailablePeriodResponse> Periods);

public sealed record VehicleUnavailablePeriodResponse(
    DateOnly StartDate,
    DateOnly EndDate);
