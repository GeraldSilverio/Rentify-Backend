using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Services;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.ReplaceVehicleFeatures;

public sealed class ReplaceVehicleFeaturesHandler
    : IRequestHandler<ReplaceVehicleFeaturesCommand, ResultReponse<bool>>
{
    private readonly IVehicleService _vehicleService;

    public ReplaceVehicleFeaturesHandler(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    public async Task<ResultReponse<bool>> Handle(
        ReplaceVehicleFeaturesCommand request,
        CancellationToken cancellationToken)
    {
        await _vehicleService.ReplaceFeaturesAsync(
            request.TenantId,
            request.VehicleId,
            request.FeatureIds,
            request.ModifiedBy,
            cancellationToken);

        return ResultReponse<bool>.Success(true);
    }
}
