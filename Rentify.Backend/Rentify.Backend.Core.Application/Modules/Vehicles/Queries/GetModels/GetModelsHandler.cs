using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetModels;

public sealed class GetModelsHandler
    : IRequestHandler<GetModelsQuery, ResultReponse<IReadOnlyCollection<ModelResponse>>>
{
    private readonly IVehicleCatalogRepository _vehicleCatalogRepository;

    public GetModelsHandler(IVehicleCatalogRepository vehicleCatalogRepository)
    {
        _vehicleCatalogRepository = vehicleCatalogRepository;
    }

    public async Task<ResultReponse<IReadOnlyCollection<ModelResponse>>> Handle(
        GetModelsQuery request,
        CancellationToken cancellationToken)
    {
        bool brandExists = await _vehicleCatalogRepository.BrandExistsAsync(request.BrandId, cancellationToken);
        if (!brandExists)
        {
            return ResultReponse<IReadOnlyCollection<ModelResponse>>.Failure(
                Error.SetError("Brand not found.", StatusCodes.Status404NotFound));
        }

        IReadOnlyCollection<ModelResponse> models =
            await _vehicleCatalogRepository.GetModelsByBrandAsync(request.BrandId, cancellationToken);

        return ResultReponse<IReadOnlyCollection<ModelResponse>>.Success(models);
    }
}
