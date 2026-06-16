using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;
using Rentify.Backend.Infraestructure.Persistence.Context;

namespace Rentify.Backend.Infraestructure.Persistence.Repositories;

public sealed class VehicleCatalogRepository : IVehicleCatalogRepository
{
    private readonly RentifyContext _context;

    public VehicleCatalogRepository(RentifyContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<VehicleBrandResponse>> GetVehicleBrandsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.VehicleBrands
            .AsNoTracking()
            .Where(x => x.IsActive && !x.IsDeleted)
            .OrderBy(x => x.Name)
            .Select(x => new VehicleBrandResponse(x.Id, x.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<VehicleModelResponse>> GetVehicleModelsByBrandAsync(
        Guid vehicleBrandId,
        CancellationToken cancellationToken = default)
    {
        return await _context.VehicleModels
            .AsNoTracking()
            .Where(x => x.VehicleBrandId == vehicleBrandId && x.IsActive && !x.IsDeleted)
            .OrderBy(x => x.Name)
            .Select(x => new VehicleModelResponse(x.Id, x.Name, x.VehicleBrandId, x.VehicleBrand.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> VehicleBrandExistsAsync(Guid vehicleBrandId, CancellationToken cancellationToken = default)
    {
        return await _context.VehicleBrands
            .AsNoTracking()
            .AnyAsync(x => x.Id == vehicleBrandId && x.IsActive && !x.IsDeleted, cancellationToken);
    }
}
