using MediatR;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Services;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.BlockVehicleAvailability;

public sealed class BlockVehicleAvailabilityHandler : IRequestHandler<BlockVehicleAvailabilityCommand, ResultReponse<bool>>
{
    private readonly IVehicleService _vehicleService;

    public BlockVehicleAvailabilityHandler(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    public async Task<ResultReponse<bool>> Handle(BlockVehicleAvailabilityCommand request, CancellationToken cancellationToken)
    {
        await _vehicleService.BlockAvailabilityAsync(request, cancellationToken);

        return ResultReponse<bool>.Success(true);
    }
}
