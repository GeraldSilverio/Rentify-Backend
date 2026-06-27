using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Services;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UpdateVehicle;

public sealed class UpdateVehicleHandler : IRequestHandler<UpdateVehicleCommand, ResultReponse<Guid>>
{
    private readonly IVehicleService _vehicleService;

    public UpdateVehicleHandler(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    public async Task<ResultReponse<Guid>> Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
    {
        await _vehicleService.UpdateAsync(request, cancellationToken);

        return ResultReponse<Guid>.Success(request.VehicleId);
    }
}
