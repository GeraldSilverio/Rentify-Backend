using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;
using Rentify.Backend.Core.Domain.Entities.Vehicles;
using Rentify.Backend.Infraestructure.Persistence.Context;

namespace Rentify.Backend.Infraestructure.Persistence.Repositories.Vehicules;

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
        return await GetVehicleBrandsAsync(true, cancellationToken);
    }

    public async Task<IReadOnlyCollection<VehicleBrandResponse>> GetVehicleBrandsAsync(
        bool onlyActive,
        CancellationToken cancellationToken = default)
    {
        IQueryable<VehicleBrand> query = _context.VehicleBrands
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        if (onlyActive)
            query = query.Where(x => x.IsActive);

        return await query
            .OrderBy(x => x.Name)
            .Select(x => new VehicleBrandResponse(x.Id, x.Name, x.IsActive))
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
            .Select(x => new VehicleModelResponse(x.Id, x.Name, x.VehicleBrandId, x.VehicleBrand.Name, x.IsActive))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<VehicleModelResponse>> GetVehicleModelsAsync(
        bool onlyActive,
        CancellationToken cancellationToken = default)
    {
        IQueryable<VehicleModel> query = _context.VehicleModels
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        if (onlyActive)
            query = query.Where(x => x.IsActive && x.VehicleBrand.IsActive && !x.VehicleBrand.IsDeleted);

        return await query
            .OrderBy(x => x.VehicleBrand.Name)
            .ThenBy(x => x.Name)
            .Select(x => new VehicleModelResponse(x.Id, x.Name, x.VehicleBrandId, x.VehicleBrand.Name, x.IsActive))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<VehicleTypeResponse>> GetVehicleTypesAsync(
        bool onlyActive,
        CancellationToken cancellationToken = default)
    {
        IQueryable<VehicleType> query = _context.VehicleTypes
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        if (onlyActive)
            query = query.Where(x => x.IsActive);

        return await query
            .OrderBy(x => x.Name)
            .Select(x => new VehicleTypeResponse(x.Id, x.Name, x.IsActive))
            .ToListAsync(cancellationToken);
    }

        public async Task<bool> VehicleModelExistsAsync(Guid vehicleModelId, CancellationToken cancellationToken = default)
    {
        return await _context.VehicleModels.AnyAsync(x => x.Id == vehicleModelId && !x.IsDeleted, cancellationToken);
    }

    public async Task<bool> VehicleTypeExistsAsync(
        Guid vehicleTypeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.VehicleTypes.AnyAsync(x=>
            x.Id == vehicleTypeId && !x.IsDeleted,
            cancellationToken);
    }

    public async Task<bool> VehicleBrandExistsAsync(Guid vehicleBrandId, CancellationToken cancellationToken = default)
    {
        return await _context.VehicleBrands
            .AsNoTracking()
            .AnyAsync(x => x.Id == vehicleBrandId && x.IsActive && !x.IsDeleted, cancellationToken);
    }

    public async Task<VehicleBrand?> GetBrandByIdAsync(Guid brandId, CancellationToken cancellationToken = default)
    {
        return await _context.VehicleBrands.FirstOrDefaultAsync(x => x.Id == brandId && !x.IsDeleted, cancellationToken);
    }

    public async Task<VehicleModel?> GetModelByIdAsync(Guid modelId, CancellationToken cancellationToken = default)
    {
        return await _context.VehicleModels.FirstOrDefaultAsync(x => x.Id == modelId && !x.IsDeleted, cancellationToken);
    }

    public async Task<VehicleType?> GetTypeByIdAsync(Guid typeId, CancellationToken cancellationToken = default)
    {
        return await _context.VehicleTypes.FirstOrDefaultAsync(x => x.Id == typeId && !x.IsDeleted, cancellationToken);
    }

    public async Task AddBrandAsync(VehicleBrand brand, CancellationToken cancellationToken = default)
    {
        await _context.VehicleBrands.AddAsync(brand, cancellationToken);
    }

    public async Task AddModelAsync(VehicleModel model, CancellationToken cancellationToken = default)
    {
        await _context.VehicleModels.AddAsync(model, cancellationToken);
    }

    public async Task AddTypeAsync(VehicleType type, CancellationToken cancellationToken = default)
    {
        await _context.VehicleTypes.AddAsync(type, cancellationToken);
    }

    public async Task<bool> BrandNameExistsAsync(
        string name,
        Guid? excludedBrandId = null,
        CancellationToken cancellationToken = default)
    {
        string normalizedName = name.Trim();
        return await _context.VehicleBrands
            .AsNoTracking()
            .AnyAsync(x => !x.IsDeleted
                           && x.Name.ToUpper() == normalizedName.ToUpper()
                           && (!excludedBrandId.HasValue || x.Id != excludedBrandId.Value),
                cancellationToken);
    }

    public async Task<bool> ModelNameExistsAsync(
        Guid brandId,
        string name,
        Guid? excludedModelId = null,
        CancellationToken cancellationToken = default)
    {
        string normalizedName = name.Trim();
        return await _context.VehicleModels
            .AsNoTracking()
            .AnyAsync(x => !x.IsDeleted
                           && x.VehicleBrandId == brandId
                           && x.Name.ToUpper() == normalizedName.ToUpper()
                           && (!excludedModelId.HasValue || x.Id != excludedModelId.Value),
                cancellationToken);
    }

    public async Task<bool> TypeNameExistsAsync(
        string name,
        Guid? excludedTypeId = null,
        CancellationToken cancellationToken = default)
    {
        string normalizedName = name.Trim();
        return await _context.VehicleTypes
            .AsNoTracking()
            .AnyAsync(x => !x.IsDeleted
                           && x.Name.ToUpper() == normalizedName.ToUpper()
                           && (!excludedTypeId.HasValue || x.Id != excludedTypeId.Value),
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<VehicleFeatureResponse>> GetVehicleFeaturesAsync(
        bool onlyActive,
        CancellationToken cancellationToken = default)
    {
        IQueryable<VehicleFeature> query = _context.VehicleFeatures
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        if (onlyActive)
            query = query.Where(x => x.IsActive);

        return await query
            .OrderBy(x => x.Category)
            .ThenBy(x => x.Name)
            .Select(x => new VehicleFeatureResponse(x.Id, x.Name, x.Category, x.IsActive))
            .ToListAsync(cancellationToken);
    }

    public async Task<VehicleFeature?> GetFeatureByIdAsync(
        Guid featureId,
        CancellationToken cancellationToken = default)
    {
        return await _context.VehicleFeatures
            .FirstOrDefaultAsync(x => x.Id == featureId && !x.IsDeleted, cancellationToken);
    }

    public async Task AddFeatureAsync(
        VehicleFeature feature,
        CancellationToken cancellationToken = default)
    {
        await _context.VehicleFeatures.AddAsync(feature, cancellationToken);
    }

    public async Task<bool> FeatureNameExistsAsync(
        string name,
        Guid? excludedFeatureId = null,
        CancellationToken cancellationToken = default)
    {
        string normalizedName = name.Trim();

        return await _context.VehicleFeatures
            .AsNoTracking()
            .AnyAsync(
                x => !x.IsDeleted
                     && x.Name.ToUpper() == normalizedName.ToUpper()
                     && (!excludedFeatureId.HasValue || x.Id != excludedFeatureId.Value),
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<Guid>> GetActiveFeatureIdsAsync(
        IReadOnlyCollection<Guid> featureIds,
        CancellationToken cancellationToken = default)
    {
        Guid[] ids = featureIds.Distinct().ToArray();

        return await _context.VehicleFeatures
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id) && x.IsActive && !x.IsDeleted)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);
    }
}
