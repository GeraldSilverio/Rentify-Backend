namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.CreateVehicle;

public sealed record CreateVehicleRequest(
    Guid RentCarId,
    Guid VehicleBrandId,
    Guid VehicleModelId,
    Guid VehicleTypeId,
    int Year,
    string PlateNumber,
    string? Vin,
    string Color,
    decimal DailyRate,
    int? CurrentMileage,
    string CreatedBy);
