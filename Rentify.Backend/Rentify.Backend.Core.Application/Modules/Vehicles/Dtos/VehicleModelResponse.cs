namespace Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;

public sealed record VehicleModelResponse(
    Guid Id,
    string Name,
    Guid VehicleBrandId,
    string VehicleBrandName,
    bool IsActive = true);
