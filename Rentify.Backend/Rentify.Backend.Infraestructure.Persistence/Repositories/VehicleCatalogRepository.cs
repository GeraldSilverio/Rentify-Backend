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

    public async Task<IReadOnlyCollection<BrandResponse>> GetBrandsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Brands
            .AsNoTracking()
            .Where(x => x.IsActive && !x.IsDeleted)
            .OrderBy(x => x.Name)
            .Select(x => new BrandResponse(x.Id, x.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ModelResponse>> GetModelsByBrandAsync(
        Guid brandId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Models
            .AsNoTracking()
            .Where(x => x.BrandId == brandId && x.IsActive && !x.IsDeleted)
            .OrderBy(x => x.Name)
            .Select(x => new ModelResponse(x.Id, x.Name, x.BrandId, x.Brand.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> BrandExistsAsync(Guid brandId, CancellationToken cancellationToken = default)
    {
        return await _context.Brands
            .AsNoTracking()
            .AnyAsync(x => x.Id == brandId && x.IsActive && !x.IsDeleted, cancellationToken);
    }
}
