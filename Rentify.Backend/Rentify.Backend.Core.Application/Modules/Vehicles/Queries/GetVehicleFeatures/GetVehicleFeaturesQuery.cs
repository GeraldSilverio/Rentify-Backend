using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicleFeatures;

public sealed record GetVehicleFeaturesQuery(
    bool OnlyActive = true) : IRequest<ResultReponse<IReadOnlyCollection<VehicleFeatureResponse>>>;
