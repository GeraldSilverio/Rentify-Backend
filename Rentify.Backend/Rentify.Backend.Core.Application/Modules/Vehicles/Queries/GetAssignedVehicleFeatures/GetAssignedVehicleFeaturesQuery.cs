using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetAssignedVehicleFeatures;

public sealed record GetAssignedVehicleFeaturesQuery(
    Guid TenantId,
    Guid VehicleId) : IRequest<ResultReponse<IReadOnlyCollection<VehicleFeatureResponse>>>;
