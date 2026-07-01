using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.BlockVehicleAvailability;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.ChangeVehicleStatus;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.CreateVehicle;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.DeleteVehicle;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.SetPrimaryVehicleImage;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UpdateVehicle;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UploadVehicleImage;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Services;

public interface IVehicleService
{
    Task<CreateVehicleResponse> CreateAsync(CreateVehicleCommand command, CancellationToken cancellationToken = default);
    Task UpdateAsync(UpdateVehicleCommand command, CancellationToken cancellationToken = default);
    Task DeleteAsync(DeleteVehicleCommand command, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<VehicleImageResponse>> GetImagesAsync(Guid tenantId, Guid vehicleId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<VehicleImageResponse>> UploadImagesAsync(UploadVehicleImageCommand command, CancellationToken cancellationToken = default);
    Task DeleteImageAsync(Guid tenantId, Guid vehicleId, Guid imageId, string modifiedBy, CancellationToken cancellationToken = default);
    Task SetPrimaryImageAsync(SetPrimaryVehicleImageCommand command, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<VehicleFeatureResponse>> GetFeaturesAsync(Guid tenantId, Guid vehicleId, CancellationToken cancellationToken = default);
    Task ReplaceFeaturesAsync(Guid tenantId, Guid vehicleId, IReadOnlyCollection<Guid> featureIds, string modifiedBy, CancellationToken cancellationToken = default);
    Task ChangeStatusAsync(ChangeVehicleStatusCommand command, CancellationToken cancellationToken = default);
    Task BlockAvailabilityAsync(BlockVehicleAvailabilityCommand command, CancellationToken cancellationToken = default);
}
