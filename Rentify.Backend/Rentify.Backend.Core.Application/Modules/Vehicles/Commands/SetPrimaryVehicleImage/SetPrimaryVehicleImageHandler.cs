using MediatR;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Services;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.SetPrimaryVehicleImage;

public sealed class SetPrimaryVehicleImageHandler
    : IRequestHandler<SetPrimaryVehicleImageCommand, ResultReponse<bool>>
{
    private readonly IVehicleService _vehicleService;

    public SetPrimaryVehicleImageHandler(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    public async Task<ResultReponse<bool>> Handle(
        SetPrimaryVehicleImageCommand request,
        CancellationToken cancellationToken)
    {
        await _vehicleService.SetPrimaryImageAsync(request, cancellationToken);

        return ResultReponse<bool>.Success(true);
    }
}
