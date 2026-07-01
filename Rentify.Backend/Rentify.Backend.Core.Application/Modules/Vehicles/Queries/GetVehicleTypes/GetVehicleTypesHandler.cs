using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicleTypes;

public sealed class GetVehicleTypesHandler
    : IRequestHandler<GetVehicleTypesQuery, ResultReponse<IReadOnlyCollection<VehicleTypeResponse>>>
{
    private readonly IVehicleCatalogRepository _catalogRepository;

    public GetVehicleTypesHandler(IVehicleCatalogRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    public async Task<ResultReponse<IReadOnlyCollection<VehicleTypeResponse>>> Handle(
        GetVehicleTypesQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<VehicleTypeResponse> types = await _catalogRepository.GetVehicleTypesAsync(
            request.OnlyActive,
            cancellationToken);

        return ResultReponse<IReadOnlyCollection<VehicleTypeResponse>>.Success(types);
    }
}
