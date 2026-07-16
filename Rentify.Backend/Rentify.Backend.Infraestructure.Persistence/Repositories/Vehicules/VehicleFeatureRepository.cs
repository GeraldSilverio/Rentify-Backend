using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Domain.Entities.Vehicles;
using Rentify.Backend.Infraestructure.Persistence.Context;

namespace Rentify.Backend.Infraestructure.Persistence.Repositories.Vehicules;

public sealed class VehicleFeatureRepository : IVehicleFeatureRepository
{
    private readonly RentifyContext _context;

    public VehicleFeatureRepository(RentifyContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<VehicleFeature>> GetActiveByIdsAsync(
        IReadOnlyCollection<Guid> featureIds,
        CancellationToken cancellationToken = default)
    {
        return await _context.VehicleFeatures
            .AsNoTracking()
            .Where(feature => featureIds.Contains(feature.Id) && feature.IsActive && !feature.IsDeleted)
            .ToListAsync(cancellationToken);
    }
}
