using MediatR;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Services;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.DeleteVehicle;

public sealed class DeleteVehicleHandler : IRequestHandler<DeleteVehicleCommand, ResultReponse<bool>>
{
    private readonly IVehicleService _vehicleService;

    public DeleteVehicleHandler(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    public async Task<ResultReponse<bool>> Handle(DeleteVehicleCommand request, CancellationToken cancellationToken)
    {
        await _vehicleService.DeleteAsync(request, cancellationToken);

        return ResultReponse<bool>.Success(true);
    }
}
