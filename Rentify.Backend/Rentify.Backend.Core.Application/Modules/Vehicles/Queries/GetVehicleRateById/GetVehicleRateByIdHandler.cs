using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;
using Rentify.Backend.Core.Domain.Entities.Vehicles;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicleRateById;

public sealed class GetVehicleRateByIdHandler
    : IRequestHandler<GetVehicleRateByIdQuery, ResultReponse<VehicleRateResponse>>
{
    private readonly IVehicleRateRepository _vehicleRateRepository;

    public GetVehicleRateByIdHandler(IVehicleRateRepository vehicleRateRepository)
    {
        _vehicleRateRepository = vehicleRateRepository;
    }

    public async Task<ResultReponse<VehicleRateResponse>> Handle(GetVehicleRateByIdQuery request, CancellationToken cancellationToken)
    {
        VehicleRate rate = await _vehicleRateRepository.GetByIdAsync(request.TenantId, request.VehicleId, request.RateId, cancellationToken)
            ?? throw new ApiException("Tarifa no encontrada.", StatusCodes.Status404NotFound);

        return ResultReponse<VehicleRateResponse>.Success(new VehicleRateResponse(rate.Id, rate.RentalType, rate.Price));
    }
}
