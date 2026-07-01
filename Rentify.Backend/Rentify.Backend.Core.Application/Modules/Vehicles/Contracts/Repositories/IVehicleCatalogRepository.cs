using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;
using Rentify.Backend.Core.Domain.Entities.Vehicles;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;

public interface IVehicleCatalogRepository
{
    Task<IReadOnlyCollection<VehicleBrandResponse>> GetVehicleBrandsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<VehicleModelResponse>> GetVehicleModelsByBrandAsync(Guid vehicleBrandId, CancellationToken cancellationToken = default);
    Task<bool> VehicleBrandExistsAsync(Guid vehicleBrandId, CancellationToken cancellationToken = default);
    Task<bool> VehicleModelExistsAsync(Guid vehicleModelId, CancellationToken cancellationToken = default);
    Task<bool> VehicleTypeExistsAsync(Guid vehicleTypeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<VehicleFeatureResponse>> GetVehicleFeaturesAsync(bool onlyActive, CancellationToken cancellationToken = default);
    Task<VehicleFeature?> GetFeatureByIdAsync(Guid featureId, CancellationToken cancellationToken = default);
    Task AddFeatureAsync(VehicleFeature feature, CancellationToken cancellationToken = default);
    Task<bool> FeatureNameExistsAsync(string name, Guid? excludedFeatureId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Guid>> GetActiveFeatureIdsAsync(IReadOnlyCollection<Guid> featureIds, CancellationToken cancellationToken = default);
}
