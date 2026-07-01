using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicleFeatures;

public sealed class GetVehicleFeaturesHandler
    : IRequestHandler<GetVehicleFeaturesQuery, ResultReponse<IReadOnlyCollection<VehicleFeatureResponse>>>
{
    private readonly IVehicleCatalogRepository _catalogRepository;

    public GetVehicleFeaturesHandler(IVehicleCatalogRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    public async Task<ResultReponse<IReadOnlyCollection<VehicleFeatureResponse>>> Handle(
        GetVehicleFeaturesQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<VehicleFeatureResponse> features = await _catalogRepository.GetVehicleFeaturesAsync(
            request.OnlyActive,
            cancellationToken);

        return ResultReponse<IReadOnlyCollection<VehicleFeatureResponse>>.Success(features);
    }
}
