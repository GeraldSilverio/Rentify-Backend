using MediatR;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetBrands;

public sealed class GetBrandsHandler
    : IRequestHandler<GetBrandsQuery, ResultReponse<IReadOnlyCollection<BrandResponse>>>
{
    private readonly IVehicleCatalogRepository _vehicleCatalogRepository;

    public GetBrandsHandler(IVehicleCatalogRepository vehicleCatalogRepository)
    {
        _vehicleCatalogRepository = vehicleCatalogRepository;
    }

    public async Task<ResultReponse<IReadOnlyCollection<BrandResponse>>> Handle(
        GetBrandsQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<BrandResponse> brands =
            await _vehicleCatalogRepository.GetBrandsAsync(cancellationToken);

        return ResultReponse<IReadOnlyCollection<BrandResponse>>.Success(brands);
    }
}
