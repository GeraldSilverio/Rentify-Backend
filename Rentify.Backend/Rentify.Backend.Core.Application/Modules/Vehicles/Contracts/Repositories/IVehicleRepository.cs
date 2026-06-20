using Rentify.Backend.Core.Domain.Entities;
using Rentify.Backend.Core.Domain.Entities.Vehicles;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;

public interface IVehicleRepository
{
    Task AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default);
    Task<Vehicle?> GetByIdAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default);
    Task<Vehicle?> GetByIdWithImagesAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default);
    Task<bool> VehicleModelExistsAsync(Guid vehicleModelId, CancellationToken cancellationToken = default);
    Task<bool> VehicleTypeExistsAsync(Guid tenantId, Guid vehicleTypeId, CancellationToken cancellationToken = default);
    Task<bool> PlateNumberExistsAsync(Guid tenantId, string plateNumber, Guid? excludedVehicleId = null, CancellationToken cancellationToken = default);
    Task<bool> VinExistsAsync(Guid tenantId, string vin, Guid? excludedVehicleId = null, CancellationToken cancellationToken = default);
}
