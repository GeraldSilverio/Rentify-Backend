using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.BlockVehicleAvailability;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.ChangeVehicleStatus;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.CreateVehicle;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Services;

public interface IVehicleService
{
    Task<Guid> CreateAsync(CreateVehicleCommand command, CancellationToken cancellationToken = default);
    Task ChangeStatusAsync(ChangeVehicleStatusCommand command, CancellationToken cancellationToken = default);
    Task BlockAvailabilityAsync(BlockVehicleAvailabilityCommand command, CancellationToken cancellationToken = default);
}
