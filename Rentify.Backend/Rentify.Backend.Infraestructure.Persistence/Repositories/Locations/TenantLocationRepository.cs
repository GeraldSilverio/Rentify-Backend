using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Core.Application.Modules.Locations.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Locations.Dtos;
using Rentify.Backend.Core.Application.Modules.Locations.Queries.GetTenantLocations;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Domain.Entities.Locations;
using Rentify.Backend.Infraestructure.Persistence.Context;

namespace Rentify.Backend.Infraestructure.Persistence.Repositories.Locations;

public sealed class TenantLocationRepository : ITenantLocationRepository
{
    private readonly RentifyContext _context;

    public TenantLocationRepository(RentifyContext context)
    {
        _context = context;
    }

    public async Task AddAsync(TenantLocation tenantLocation, CancellationToken cancellationToken = default)
    {
        await _context.TenantLocations.AddAsync(tenantLocation, cancellationToken);
    }

    public async Task<TenantLocation?> GetByIdAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.TenantLocations
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<TenantLocationResponse?> GetDetailsAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.TenantLocations
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.Id == id && !x.IsDeleted)
            .Select(x => new TenantLocationResponse(
                x.Id,
                x.LocationId,
                x.DisplayName,
                x.Location == null ? null : x.Location.Type,
                x.Location == null ? null : x.Location.City,
                x.Location == null ? null : x.Location.Province,
                x.Location == null ? null : x.Location.Country,
                x.Location == null ? null : x.Location.Address,
                x.AllowsDelivery,
                x.AllowsPickup,
                x.DeliveryFee,
                x.PickupFee,
                x.IsCustom,
                x.IsActive))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PaginatedResponse<TenantLocationListItemResponse>> GetPagedAsync(
        GetTenantLocationsQuery query,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TenantLocation> tenantLocationsQuery = _context.TenantLocations
            .AsNoTracking()
            .Where(x => x.TenantId == query.TenantId && !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string searchPattern = $"%{query.Search.Trim()}%";
            tenantLocationsQuery = tenantLocationsQuery.Where(x =>
                EF.Functions.ILike(x.DisplayName, searchPattern)
                || (x.Location != null && x.Location.City != null && EF.Functions.ILike(x.Location.City, searchPattern))
                || (x.Location != null && x.Location.Province != null && EF.Functions.ILike(x.Location.Province, searchPattern)));
        }

        if (query.IsActive.HasValue)
            tenantLocationsQuery = tenantLocationsQuery.Where(x => x.IsActive == query.IsActive.Value);

        if (query.SupportsDelivery.HasValue)
            tenantLocationsQuery = tenantLocationsQuery.Where(x => x.AllowsDelivery == query.SupportsDelivery.Value);

        if (query.SupportsPickup.HasValue)
            tenantLocationsQuery = tenantLocationsQuery.Where(x => x.AllowsPickup == query.SupportsPickup.Value);

        int totalCount = await tenantLocationsQuery.CountAsync(cancellationToken);
        int totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)query.PageSize);

        List<TenantLocationListItemResponse> tenantLocations = await tenantLocationsQuery
            .OrderBy(x => x.DisplayName)
            .ThenBy(x => x.Id)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => new TenantLocationListItemResponse(
                x.Id,
                x.LocationId,
                x.DisplayName,
                x.Location == null ? null : x.Location.Type,
                x.Location == null ? null : x.Location.City,
                x.Location == null ? null : x.Location.Province,
                x.AllowsDelivery,
                x.AllowsPickup,
                x.DeliveryFee,
                x.PickupFee,
                x.IsCustom,
                x.IsActive))
            .ToListAsync(cancellationToken);

        return new PaginatedResponse<TenantLocationListItemResponse>(
            tenantLocations,
            query.PageNumber,
            query.PageSize,
            totalCount,
            totalPages);
    }

    public async Task<bool> ExistsByLocationAsync(
        Guid tenantId,
        Guid locationId,
        Guid? excludeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.TenantLocations.AnyAsync(x =>
            x.TenantId == tenantId
            && x.LocationId == locationId
            && !x.IsDeleted
            && (!excludeId.HasValue || x.Id != excludeId.Value),
            cancellationToken);
    }

    public async Task<bool> ExistsByDisplayNameAsync(
        Guid tenantId,
        string displayName,
        Guid? excludeId,
        CancellationToken cancellationToken = default)
    {
        string normalizedDisplayName = displayName.Trim();

        return await _context.TenantLocations.AnyAsync(x =>
            x.TenantId == tenantId
            && x.DisplayName == normalizedDisplayName
            && !x.IsDeleted
            && (!excludeId.HasValue || x.Id != excludeId.Value),
            cancellationToken);
    }
}
