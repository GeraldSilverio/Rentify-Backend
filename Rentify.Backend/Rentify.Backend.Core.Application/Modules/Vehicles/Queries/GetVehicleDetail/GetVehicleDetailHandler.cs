using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicleDetail;

public sealed class GetVehicleDetailHandler
    : IRequestHandler<GetVehicleDetailQuery, ResultReponse<VehicleDetailResponse>>
{
    private readonly IVehicleRepository _vehicleRepository;

    public GetVehicleDetailHandler(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task<ResultReponse<VehicleDetailResponse>> Handle(
        GetVehicleDetailQuery request,
        CancellationToken cancellationToken)
    {
        VehicleDetailResponse response = await _vehicleRepository.GetDetailAsync(
            request.TenantId,
            request.VehicleId,
            cancellationToken)
            ?? throw new ApiException("Vehicle not found.", StatusCodes.Status404NotFound);

        return ResultReponse<VehicleDetailResponse>.Success(response);
    }
}
