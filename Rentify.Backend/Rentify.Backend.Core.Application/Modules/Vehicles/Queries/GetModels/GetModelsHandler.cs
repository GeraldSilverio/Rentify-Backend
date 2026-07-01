using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetModels;

public sealed class GetModelsHandler
    : IRequestHandler<GetModelsQuery, ResultReponse<IReadOnlyCollection<VehicleModelResponse>>>
{
    private readonly IVehicleCatalogRepository _vehicleCatalogRepository;

    public GetModelsHandler(IVehicleCatalogRepository vehicleCatalogRepository)
    {
        _vehicleCatalogRepository = vehicleCatalogRepository;
    }

    public async Task<ResultReponse<IReadOnlyCollection<VehicleModelResponse>>> Handle(
        GetModelsQuery request,
        CancellationToken cancellationToken)
    {
        if (!request.VehicleBrandId.HasValue)
        {
            IReadOnlyCollection<VehicleModelResponse> allModels =
                await _vehicleCatalogRepository.GetVehicleModelsAsync(request.OnlyActive, cancellationToken);

            return ResultReponse<IReadOnlyCollection<VehicleModelResponse>>.Success(allModels);
        }

        bool brandExists = await _vehicleCatalogRepository.VehicleBrandExistsAsync(request.VehicleBrandId.Value, cancellationToken);
        if (!brandExists)
        {
            return ResultReponse<IReadOnlyCollection<VehicleModelResponse>>.Failure(
                Error.SetError("Vehicle brand not found.", StatusCodes.Status404NotFound));
        }

        IReadOnlyCollection<VehicleModelResponse> models =
            await _vehicleCatalogRepository.GetVehicleModelsByBrandAsync(request.VehicleBrandId.Value, cancellationToken);

        return ResultReponse<IReadOnlyCollection<VehicleModelResponse>>.Success(models);
    }
}
