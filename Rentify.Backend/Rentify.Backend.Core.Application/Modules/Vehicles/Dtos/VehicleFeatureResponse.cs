namespace Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;

public sealed record VehicleFeatureResponse(
    Guid Id,
    string Name,
    string Category,
    bool IsActive);
