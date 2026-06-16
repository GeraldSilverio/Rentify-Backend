namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UpdateVehicle;

public sealed record UpdateVehicleRequest(
    Guid VehicleModelId,
    Guid VehicleTypeId,
    int Year,
    string PlateNumber,
    string Vin,
    string Color,
    decimal DailyRate,
    string ModifiedBy);
