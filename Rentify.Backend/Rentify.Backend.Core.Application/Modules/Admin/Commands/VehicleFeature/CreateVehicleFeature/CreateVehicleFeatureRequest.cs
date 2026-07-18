namespace Rentify.Backend.Core.Application.Modules.Admin.Commands.VehicleFeature.CreateVehicleFeature;

public sealed record CreateVehicleFeatureRequest(
    string Name,
    string Category);
