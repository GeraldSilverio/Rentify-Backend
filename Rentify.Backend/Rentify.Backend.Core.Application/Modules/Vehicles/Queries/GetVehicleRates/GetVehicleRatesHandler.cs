using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicleRates;

public sealed class GetVehicleRatesHandler
    : IRequestHandler<GetVehicleRatesQuery, ResultReponse<IReadOnlyCollection<VehicleRateResponse>>>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IVehicleRateRepository _vehicleRateRepository;

    public GetVehicleRatesHandler(IVehicleRepository vehicleRepository, IVehicleRateRepository vehicleRateRepository)
    {
        _vehicleRepository = vehicleRepository;
        _vehicleRateRepository = vehicleRateRepository;
    }

    public async Task<ResultReponse<IReadOnlyCollection<VehicleRateResponse>>> Handle(GetVehicleRatesQuery request, CancellationToken cancellationToken)
    {
        await EnsureVehicleExistsAsync(request.TenantId, request.VehicleId, cancellationToken);

        IReadOnlyCollection<VehicleRateResponse> rates = (await _vehicleRateRepository.GetByVehicleIdAsync(
            request.TenantId,
            request.VehicleId,
            cancellationToken))
            .OrderBy(rate => rate.RentalType)
            .Select(rate => new VehicleRateResponse(rate.Id, rate.RentalType, rate.Price))
            .ToList();

        return ResultReponse<IReadOnlyCollection<VehicleRateResponse>>.Success(rates);
    }

    private async Task EnsureVehicleExistsAsync(Guid tenantId, Guid vehicleId, CancellationToken cancellationToken)
    {
        if (await _vehicleRepository.GetByIdAsync(tenantId, vehicleId, cancellationToken) is null)
            throw new ApiException("Vehículo no encontrado.", StatusCodes.Status404NotFound);
    }
}
