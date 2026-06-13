using MediatR;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Services;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.DeleteVehicleImage;

public sealed class DeleteVehicleImageHandler : IRequestHandler<DeleteVehicleImageCommand, ResultReponse<bool>>
{
    private readonly IVehicleService _vehicleService;

    public DeleteVehicleImageHandler(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    public async Task<ResultReponse<bool>> Handle(DeleteVehicleImageCommand request, CancellationToken cancellationToken)
    {
        await _vehicleService.DeleteImageAsync(request, cancellationToken);

        return ResultReponse<bool>.Success(true);
    }
}
