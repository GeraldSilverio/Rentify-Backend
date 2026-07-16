using Rentify.Backend.Core.Domain.Entities.Vehicles;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;

public interface IVehicleFeatureAssignmentRepository
{
    Task<IReadOnlyCollection<VehicleFeatureAssignment>> GetAssignmentsByVehicleAsync(
        Guid tenantId,
        Guid vehicleId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<VehicleFeatureAssignment>> GetAssignmentsByVehicleWithFeaturesAsync(
        Guid tenantId,
        Guid vehicleId,
        CancellationToken cancellationToken = default);

    Task AddRangeAsync(
        IReadOnlyCollection<VehicleFeatureAssignment> assignments,
        CancellationToken cancellationToken = default);

    void RemoveRange(IReadOnlyCollection<VehicleFeatureAssignment> assignments);
}
