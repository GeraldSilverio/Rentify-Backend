using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Core.Application.Modules.Locations.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Locations.Dtos;
using Rentify.Backend.Core.Application.Modules.Locations.Queries.GetAdminLocations;
using Rentify.Backend.Core.Application.Modules.Locations.Queries.GetAvailableLocations;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Domain.Entities.Locations;
using Rentify.Backend.Infrastructure.Persistence.Context;

namespace Rentify.Backend.Infrastructure.Persistence.Repositories.Locations;

public sealed class LocationRepository : ILocationRepository
{
    private readonly RentifyContext _context;

    public LocationRepository(RentifyContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Location location, CancellationToken cancellationToken = default)
    {
        await _context.Locations.AddAsync(location, cancellationToken);
    }

    public async Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Locations
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<Location?> GetActiveByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Locations
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive && !x.IsDeleted, cancellationToken);
    }

    public async Task<LocationResponse?> GetDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Locations
            .AsNoTracking()
            .Where(x => x.Id == id && !x.IsDeleted)
            .Select(x => new LocationResponse(
                x.Id,
                x.Name,
                x.Type,
                x.City,
                x.Province,
                x.Country,
                x.Address,
                x.Notes,
                x.IsActive,
                x.CreatedDate,
                x.ModifiedDate))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PaginatedResponse<LocationListItemResponse>> GetPagedAsync(
        GetAdminLocationsQuery query,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Location> locationsQuery = _context.Locations
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string searchPattern = $"%{query.Search.Trim()}%";
            locationsQuery = locationsQuery.Where(x =>
                EF.Functions.ILike(x.Name, searchPattern)
                || (x.City != null && EF.Functions.ILike(x.City, searchPattern))
                || (x.Province != null && EF.Functions.ILike(x.Province, searchPattern))
                || EF.Functions.ILike(x.Country, searchPattern));
        }

        if (query.Type.HasValue)
            locationsQuery = locationsQuery.Where(x => x.Type == query.Type.Value);

        if (query.IsActive.HasValue)
            locationsQuery = locationsQuery.Where(x => x.IsActive == query.IsActive.Value);

        int totalCount = await locationsQuery.CountAsync(cancellationToken);
        int totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)query.PageSize);

        List<LocationListItemResponse> locations = await locationsQuery
            .OrderBy(x => x.Name)
            .ThenBy(x => x.City)
            .ThenBy(x => x.Id)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => new LocationListItemResponse(
                x.Id,
                x.Name,
                x.Type,
                x.City,
                x.Province,
                x.Country,
                x.Address,
                x.IsActive))
            .ToListAsync(cancellationToken);

        return new PaginatedResponse<LocationListItemResponse>(locations, query.PageNumber, query.PageSize, totalCount, totalPages);
    }

    public async Task<IReadOnlyCollection<AvailableLocationResponse>> GetAvailableAsync(
        GetAvailableLocationsQuery query,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Location> locationsQuery = _context.Locations
            .AsNoTracking()
            .Where(x => x.IsActive && !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string searchPattern = $"%{query.Search.Trim()}%";
            locationsQuery = locationsQuery.Where(x =>
                EF.Functions.ILike(x.Name, searchPattern)
                || (x.City != null && EF.Functions.ILike(x.City, searchPattern))
                || (x.Province != null && EF.Functions.ILike(x.Province, searchPattern))
                || EF.Functions.ILike(x.Country, searchPattern));
        }

        if (query.Type.HasValue)
            locationsQuery = locationsQuery.Where(x => x.Type == query.Type.Value);

        return await locationsQuery
            .OrderBy(x => x.Name)
            .ThenBy(x => x.City)
            .ThenBy(x => x.Id)
            .Select(x => new AvailableLocationResponse(
                x.Id,
                x.Name,
                x.Type,
                x.City,
                x.Province,
                x.Country,
                x.Address))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsDuplicateAsync(
        string name,
        string? city,
        string? province,
        string? country,
        Guid? excludeId,
        CancellationToken cancellationToken = default)
    {
        string normalizedName = name.Trim();
        string? normalizedCity = NormalizeOptional(city);
        string? normalizedProvince = NormalizeOptional(province);
        string normalizedCountry = string.IsNullOrWhiteSpace(country) ? Location.DefaultCountry : country.Trim();

        return await _context.Locations.AnyAsync(x =>
            x.Name == normalizedName
            && x.City == normalizedCity
            && x.Province == normalizedProvince
            && x.Country == normalizedCountry
            && !x.IsDeleted
            && (!excludeId.HasValue || x.Id != excludeId.Value),
            cancellationToken);
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
