using MediatR;
using Microsoft.Extensions.Logging;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Services;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.BlockVehicleAvailability;

public sealed class BlockVehicleAvailabilityHandler : IRequestHandler<BlockVehicleAvailabilityCommand, ResultReponse<bool>>
{
    private readonly IVehicleService _vehicleService;
    private readonly ILogger<BlockVehicleAvailabilityHandler> _logger;

    public BlockVehicleAvailabilityHandler(
        IVehicleService vehicleService,
        ILogger<BlockVehicleAvailabilityHandler> logger)
    {
        _vehicleService = vehicleService;
        _logger = logger;
    }

    public async Task<ResultReponse<bool>> Handle(BlockVehicleAvailabilityCommand request, CancellationToken cancellationToken)
    {
        await _vehicleService.BlockAvailabilityAsync(request, cancellationToken);

        _logger.LogInformation(
            "Vehicle availability block created for Vehicle {VehicleId} from {StartDateTime} to {EndDateTime} in Tenant {TenantId} with block type {AvailabilityBlockType}",
            request.VehicleId,
            request.StartDate,
            request.EndDate,
            request.TenantId,
            "Manual");

        return ResultReponse<bool>.Success(true);
    }
}
