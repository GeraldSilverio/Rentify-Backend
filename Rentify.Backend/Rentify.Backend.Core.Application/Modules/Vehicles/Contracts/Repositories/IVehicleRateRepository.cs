using Rentify.Backend.Core.Domain.Entities.Vehicles;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;

public interface IVehicleRateRepository
{
    Task<IReadOnlyCollection<VehicleRate>> GetByVehicleIdAsync(Guid tenantId, Guid vehicleId, CancellationToken cancellationToken = default);
    Task<VehicleRate?> GetByIdAsync(Guid tenantId, Guid vehicleId, Guid rateId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByRentalTypeAsync(Guid tenantId, Guid vehicleId, RentalType rentalType, Guid? excludedRateId = null, CancellationToken cancellationToken = default);
    Task<int> CountActiveByVehicleAsync(Guid tenantId, Guid vehicleId, CancellationToken cancellationToken = default);
    Task AddAsync(VehicleRate rate, CancellationToken cancellationToken = default);
}
