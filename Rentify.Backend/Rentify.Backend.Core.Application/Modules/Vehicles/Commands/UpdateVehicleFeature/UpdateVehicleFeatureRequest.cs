namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UpdateVehicleFeature;

public sealed record UpdateVehicleFeatureRequest(
    string Name,
    string Category);
