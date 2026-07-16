namespace Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;

public sealed record VehicleFeatureAssignmentResponse(
    Guid Id,
    string Name,
    string Category);
