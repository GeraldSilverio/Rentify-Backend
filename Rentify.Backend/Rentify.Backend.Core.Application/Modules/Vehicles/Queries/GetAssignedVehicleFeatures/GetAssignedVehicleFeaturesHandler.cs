using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetAssignedVehicleFeatures;

public sealed class GetAssignedVehicleFeaturesHandler
    : IRequestHandler<GetAssignedVehicleFeaturesQuery, ResultReponse<IReadOnlyCollection<VehicleFeatureResponse>>>
{
    private readonly IVehicleService _vehicleService;

    public GetAssignedVehicleFeaturesHandler(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    public async Task<ResultReponse<IReadOnlyCollection<VehicleFeatureResponse>>> Handle(
        GetAssignedVehicleFeaturesQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<VehicleFeatureResponse> features = await _vehicleService.GetFeaturesAsync(
            request.TenantId,
            request.VehicleId,
            cancellationToken);

        return ResultReponse<IReadOnlyCollection<VehicleFeatureResponse>>.Success(features);
    }
}
