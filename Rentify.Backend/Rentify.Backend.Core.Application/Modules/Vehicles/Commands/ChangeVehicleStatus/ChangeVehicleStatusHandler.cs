using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Services;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.ChangeVehicleStatus;

public sealed class ChangeVehicleStatusHandler : IRequestHandler<ChangeVehicleStatusCommand, ResultReponse<bool>>
{
    private readonly IVehicleService _vehicleService;

    public ChangeVehicleStatusHandler(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    public async Task<ResultReponse<bool>> Handle(ChangeVehicleStatusCommand request, CancellationToken cancellationToken)
    {
        await _vehicleService.ChangeStatusAsync(request, cancellationToken);

        return ResultReponse<bool>.Success(true);
    }
}
