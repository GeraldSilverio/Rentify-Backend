namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.CreateVehicle;

public sealed record CreateVehicleRequest(
    Guid RentCarId,
    Guid VehicleModelId,
    Guid VehicleTypeId,
    int Year,
    string PlateNumber,
    string Vin,
    string Color,
    decimal DailyRate,
    string CreatedBy);
