using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;

public interface IVehicleCatalogRepository
{
    Task<IReadOnlyCollection<VehicleBrandResponse>> GetVehicleBrandsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<VehicleModelResponse>> GetVehicleModelsByBrandAsync(Guid vehicleBrandId, CancellationToken cancellationToken = default);
    Task<bool> VehicleBrandExistsAsync(Guid vehicleBrandId, CancellationToken cancellationToken = default);
    Task<bool> VehicleModelExistsAsync(Guid vehicleModelId, CancellationToken cancellationToken = default);
    Task<bool> VehicleTypeExistsAsync(Guid vehicleTypeId, CancellationToken cancellationToken = default);
}
