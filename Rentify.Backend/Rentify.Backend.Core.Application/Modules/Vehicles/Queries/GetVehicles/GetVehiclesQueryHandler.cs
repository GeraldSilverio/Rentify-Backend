using MediatR;
using Rentify.Backend.Core.Application.Common.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicles;

public sealed class GetVehiclesQueryHandler
    : IRequestHandler<GetVehiclesQuery, ResultReponse<PaginatedResponse<VehicleListItemResponse>>>
{
    private readonly IVehicleRepository _vehicleRepository;

    public GetVehiclesQueryHandler(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task<ResultReponse<PaginatedResponse<VehicleListItemResponse>>> Handle(
        GetVehiclesQuery request,
        CancellationToken cancellationToken)
    {
        PaginatedResponse<VehicleListItemResponse> response =
            await _vehicleRepository.GetPagedAsync(request, cancellationToken);

        return ResultReponse<PaginatedResponse<VehicleListItemResponse>>.Success(response);
    }
}
