using Rentify.Backend.Core.Domain.Entities.Vehicles;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;

public interface IVehicleFeatureRepository
{
    Task<IReadOnlyCollection<VehicleFeature>> GetActiveByIdsAsync(
        IReadOnlyCollection<Guid> featureIds,
        CancellationToken cancellationToken = default);
}
