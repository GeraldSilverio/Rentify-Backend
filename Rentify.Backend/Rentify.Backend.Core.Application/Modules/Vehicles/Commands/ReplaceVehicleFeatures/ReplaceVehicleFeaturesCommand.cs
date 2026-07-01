using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.ReplaceVehicleFeatures;

public sealed record ReplaceVehicleFeaturesCommand(
    Guid TenantId,
    Guid VehicleId,
    IReadOnlyCollection<Guid> FeatureIds,
    string ModifiedBy) : IRequest<ResultReponse<bool>>;
