using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UpdateVehicleFeatures;

public sealed record UpdateVehicleFeaturesCommand(
    Guid TenantId,
    Guid VehicleId,
    IReadOnlyCollection<Guid> FeatureIds,
    string ModifiedBy) : IRequest<ResultReponse<IReadOnlyCollection<VehicleFeatureAssignmentResponse>>>;
