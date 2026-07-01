namespace Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;

public sealed record VehicleTypeResponse(
    Guid Id,
    string Name,
    bool IsActive);
