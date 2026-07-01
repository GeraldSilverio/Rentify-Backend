using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Services;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.ChangeVehicleActivation;

public sealed class ChangeVehicleActivationHandler
    : IRequestHandler<ChangeVehicleActivationCommand, ResultReponse<bool>>
{
    private readonly IVehicleService _vehicleService;

    public ChangeVehicleActivationHandler(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    public async Task<ResultReponse<bool>> Handle(
        ChangeVehicleActivationCommand request,
        CancellationToken cancellationToken)
    {
        await _vehicleService.ChangeActivationAsync(
            request.TenantId,
            request.VehicleId,
            request.IsActive,
            request.ModifiedBy,
            cancellationToken);

        return ResultReponse<bool>.Success(true);
    }
}
