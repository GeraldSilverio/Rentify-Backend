namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.ReplaceVehicleFeatures;

public sealed record ReplaceVehicleFeaturesRequest(
    IReadOnlyCollection<Guid> FeatureIds);
