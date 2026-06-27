using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Services;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.CreateVehicle;

public sealed class CreateVehicleHandler : IRequestHandler<CreateVehicleCommand, ResultReponse<Guid>>
{
    private readonly IVehicleService _vehicleService;

    public CreateVehicleHandler(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    public async Task<ResultReponse<Guid>> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        Guid vehicleId = await _vehicleService.CreateAsync(request, cancellationToken);

        return ResultReponse<Guid>.Success(vehicleId);
    }
}
