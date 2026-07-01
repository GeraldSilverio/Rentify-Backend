using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Services;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.CreateVehicle;

public sealed class CreateVehicleHandler : IRequestHandler<CreateVehicleCommand, ResultReponse<CreateVehicleResponse>>
{
    private readonly IVehicleService _vehicleService;

    public CreateVehicleHandler(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    public async Task<ResultReponse<CreateVehicleResponse>> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        CreateVehicleResponse response = await _vehicleService.CreateAsync(request, cancellationToken);

        return ResultReponse<CreateVehicleResponse>.Success(response);
    }
}
