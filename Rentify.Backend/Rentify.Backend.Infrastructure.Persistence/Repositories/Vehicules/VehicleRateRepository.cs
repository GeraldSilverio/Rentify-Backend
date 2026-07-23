using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Domain.Entities.Vehicles;
using Rentify.Backend.Core.Domain.Enums;
using Rentify.Backend.Infrastructure.Persistence.Context;

namespace Rentify.Backend.Infrastructure.Persistence.Repositories.Vehicules;

public sealed class VehicleRateRepository : IVehicleRateRepository
{
    private readonly RentifyContext _context;

    public VehicleRateRepository(RentifyContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<VehicleRate>> GetByVehicleIdAsync(Guid tenantId, Guid vehicleId, CancellationToken cancellationToken = default)
    {
        return await _context.VehicleRates
            .AsNoTracking()
            .Where(rate => rate.TenantId == tenantId
                           && rate.VehicleId == vehicleId
                           && rate.IsActive
                           && !rate.IsDeleted)
            .OrderBy(rate => rate.RentalType)
            .ToListAsync(cancellationToken);
    }

    public async Task<VehicleRate?> GetByIdAsync(Guid tenantId, Guid vehicleId, Guid rateId, CancellationToken cancellationToken = default)
    {
        return await _context.VehicleRates.FirstOrDefaultAsync(
            rate => rate.TenantId == tenantId
                    && rate.VehicleId == vehicleId
                    && rate.Id == rateId
                    && rate.IsActive
                    && !rate.IsDeleted,
            cancellationToken);
    }

    public Task<bool> ExistsByRentalTypeAsync(Guid tenantId, Guid vehicleId, RentalType rentalType, Guid? excludedRateId = null, CancellationToken cancellationToken = default)
    {
        return _context.VehicleRates.AnyAsync(
            rate => rate.TenantId == tenantId
                    && rate.VehicleId == vehicleId
                    && rate.RentalType == rentalType
                    && rate.IsActive
                    && !rate.IsDeleted
                    && (!excludedRateId.HasValue || rate.Id != excludedRateId.Value),
            cancellationToken);
    }

    public Task<int> CountActiveByVehicleAsync(Guid tenantId, Guid vehicleId, CancellationToken cancellationToken = default)
    {
        return _context.VehicleRates.CountAsync(
            rate => rate.TenantId == tenantId
                    && rate.VehicleId == vehicleId
                    && rate.IsActive
                    && !rate.IsDeleted,
            cancellationToken);
    }

    public async Task AddAsync(VehicleRate rate, CancellationToken cancellationToken = default)
    {
        await _context.VehicleRates.AddAsync(rate, cancellationToken);
    }
}
