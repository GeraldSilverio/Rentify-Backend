using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicleUnavailablePeriods;

public sealed class GetVehicleUnavailablePeriodsQueryHandler
    : IRequestHandler<GetVehicleUnavailablePeriodsQuery, ResultReponse<VehicleUnavailablePeriodsResponse>>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IVehicleUnavailableDateRepository _unavailableDateRepository;

    public GetVehicleUnavailablePeriodsQueryHandler(
        IVehicleRepository vehicleRepository,
        IVehicleUnavailableDateRepository unavailableDateRepository)
    {
        _vehicleRepository = vehicleRepository;
        _unavailableDateRepository = unavailableDateRepository;
    }

    public async Task<ResultReponse<VehicleUnavailablePeriodsResponse>> Handle(
        GetVehicleUnavailablePeriodsQuery request,
        CancellationToken cancellationToken)
    {
        bool vehicleExists = await _vehicleRepository.ExistsAsync(
            request.TenantId,
            request.VehicleId,
            cancellationToken);

        if (!vehicleExists)
        {
            throw new ApiException(
                "El vehículo no fue encontrado.",
                StatusCodes.Status404NotFound,
                "VEHICLE_NOT_FOUND");
        }

        DateOnly? fromDate = request.FromDate;
        DateOnly? toDate = request.ToDate;

        if (!fromDate.HasValue && !toDate.HasValue)
            fromDate = DateOnly.FromDateTime(DateTime.UtcNow);

        IReadOnlyCollection<VehicleUnavailablePeriodResponse> periods =
            await _unavailableDateRepository.GetActivePeriodsAsync(
                request.TenantId,
                request.VehicleId,
                fromDate,
                toDate,
                cancellationToken);

        return ResultReponse<VehicleUnavailablePeriodsResponse>.Success(
            new VehicleUnavailablePeriodsResponse(request.VehicleId, periods));
    }
}
