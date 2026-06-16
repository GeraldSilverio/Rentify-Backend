using MediatR;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetBrands;

public sealed class GetBrandsHandler
    : IRequestHandler<GetBrandsQuery, ResultReponse<IReadOnlyCollection<VehicleBrandResponse>>>
{
    private readonly IVehicleCatalogRepository _vehicleCatalogRepository;

    public GetBrandsHandler(IVehicleCatalogRepository vehicleCatalogRepository)
    {
        _vehicleCatalogRepository = vehicleCatalogRepository;
    }

    public async Task<ResultReponse<IReadOnlyCollection<VehicleBrandResponse>>> Handle(
        GetBrandsQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<VehicleBrandResponse> brands =
            await _vehicleCatalogRepository.GetVehicleBrandsAsync(cancellationToken);

        return ResultReponse<IReadOnlyCollection<VehicleBrandResponse>>.Success(brands);
    }
}
