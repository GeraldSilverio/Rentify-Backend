namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.CreateVehicle;

public sealed record CreateVehicleRequest(
    Guid VehicleBrandId,
    Guid VehicleModelId,
    Guid VehicleTypeId,
    int Year,
    string PlateNumber,
    string? Vin,
    string Color,
    int? CurrentMileage,
    IReadOnlyCollection<CreateVehicleRateRequest> Rates,
    IReadOnlyCollection<Guid>? FeatureIds = null);
