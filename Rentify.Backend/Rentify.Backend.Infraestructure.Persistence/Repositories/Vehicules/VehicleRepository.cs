using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;
using Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicles;
using Rentify.Backend.Core.Domain.Entities.Vehicles;
using Rentify.Backend.Core.Domain.Enums;
using Rentify.Backend.Infraestructure.Persistence.Context;

namespace Rentify.Backend.Infraestructure.Persistence.Repositories;

public sealed class VehicleRepository : IVehicleRepository
{
    private readonly RentifyContext _context;

    public VehicleRepository(RentifyContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default)
    {
        await _context.Vehicles.AddAsync(vehicle, cancellationToken);
    }

    public async Task AddImagesAsync(IEnumerable<VehicleImage> images, CancellationToken cancellationToken = default)
    {
        await _context.VehicleImages.AddRangeAsync(images, cancellationToken);
    }

    public async Task<PaginatedResponse<VehicleListItemResponse>> GetPagedAsync(
        GetVehiclesQuery query,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Vehicle> vehiclesQuery = _context.Vehicles
            .AsNoTracking()
            .Where(vehicle => vehicle.TenantId == query.TenantId && !vehicle.IsDeleted);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string searchPattern = $"%{query.Search.Trim()}%";
            vehiclesQuery = vehiclesQuery.Where(vehicle =>
                EF.Functions.ILike(vehicle.PlateNumber, searchPattern)
                || (vehicle.Vin != null && EF.Functions.ILike(vehicle.Vin, searchPattern))
                || EF.Functions.ILike(vehicle.VehicleModel.Name, searchPattern)
                || EF.Functions.ILike(vehicle.VehicleBrand.Name, searchPattern));
        }

        if (query.VehicleTypeId.HasValue)
            vehiclesQuery = vehiclesQuery.Where(vehicle => vehicle.VehicleTypeId == query.VehicleTypeId.Value);

        if (query.VehicleBrandId.HasValue)
            vehiclesQuery = vehiclesQuery.Where(vehicle => vehicle.VehicleBrandId == query.VehicleBrandId.Value);

        if (query.VehicleModelId.HasValue)
            vehiclesQuery = vehiclesQuery.Where(vehicle => vehicle.VehicleModelId == query.VehicleModelId.Value);

        if (query.Status.HasValue)
            vehiclesQuery = vehiclesQuery.Where(vehicle => vehicle.Status == query.Status.Value);

        if (query.Year.HasValue)
            vehiclesQuery = vehiclesQuery.Where(vehicle => vehicle.Year == query.Year.Value);

        if (query.MinDailyRate.HasValue)
        {
            vehiclesQuery = vehiclesQuery.Where(vehicle => vehicle.Rates
                .Any(rate => !rate.IsDeleted
                             && rate.IsActive
                             && rate.RentalType == RentalType.Daily
                             && rate.Price >= query.MinDailyRate.Value));
        }

        if (query.MaxDailyRate.HasValue)
        {
            vehiclesQuery = vehiclesQuery.Where(vehicle => vehicle.Rates
                .Any(rate => !rate.IsDeleted
                             && rate.IsActive
                             && rate.RentalType == RentalType.Daily
                             && rate.Price <= query.MaxDailyRate.Value));
        }

        if (query.OnlyActive.HasValue)
            vehiclesQuery = vehiclesQuery.Where(vehicle => vehicle.IsActive == query.OnlyActive.Value);

        int totalCount = await vehiclesQuery.CountAsync(cancellationToken);
        int totalPages = totalCount == 0
            ? 0
            : (int)Math.Ceiling(totalCount / (double)query.PageSize);

        var vehicles = await vehiclesQuery
            .OrderByDescending(vehicle => vehicle.CreatedDate)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(vehicle => new VehicleListItemResponse(
                vehicle.Id,
                vehicle.TenantId,
                vehicle.VehicleBrandId,
                vehicle.VehicleBrand.Name,
                vehicle.VehicleModelId,
                vehicle.VehicleModel.Name,
                vehicle.VehicleTypeId,
                vehicle.VehicleType.Name,
                vehicle.Year,
                vehicle.PlateNumber,
                vehicle.Vin,
                vehicle.Color,
                vehicle.CurrentMileage,
                vehicle.Status,
                vehicle.IsActive,
                vehicle.Images
                    .Where(image => !image.IsDeleted && image.IsPrimary)
                    .Select(image => image.Url)
                    .FirstOrDefault(),
                vehicle.Rates
                    .Where(rate => !rate.IsDeleted && rate.IsActive)
                    .OrderBy(rate => rate.RentalType)
                    .Select(rate => new VehicleRateResponse(rate.RentalType, rate.Price))
                    .ToList(),
                vehicle.FeatureAssignments.Count(feature => !feature.IsDeleted),
                vehicle.CreatedDate))
            .ToListAsync(cancellationToken);

        if (vehicles.Count == 0)
        {
            return new PaginatedResponse<VehicleListItemResponse>(
                vehicles,
                query.PageNumber,
                query.PageSize,
                totalCount,
                totalPages);
        }

        return new PaginatedResponse<VehicleListItemResponse>(
            vehicles,
            query.PageNumber,
            query.PageSize,
            totalCount,
            totalPages);
    }

    public async Task<VehicleDetailResponse?> GetDetailAsync(
        Guid tenantId,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Vehicles
            .AsNoTracking()
            .Where(vehicle => vehicle.TenantId == tenantId && vehicle.Id == id && !vehicle.IsDeleted)
            .Select(vehicle => new VehicleDetailResponse(
                vehicle.Id,
                vehicle.TenantId,
                vehicle.VehicleBrandId,
                vehicle.VehicleBrand.Name,
                vehicle.VehicleModelId,
                vehicle.VehicleModel.Name,
                vehicle.VehicleTypeId,
                vehicle.VehicleType.Name,
                vehicle.Year,
                vehicle.PlateNumber,
                vehicle.Vin,
                vehicle.Color,
                vehicle.CurrentMileage,
                vehicle.Status,
                vehicle.IsActive,
                vehicle.Rates
                    .Where(rate => !rate.IsDeleted)
                    .OrderBy(rate => rate.RentalType)
                    .Select(rate => new VehicleRateResponse(rate.RentalType, rate.Price))
                    .ToList(),
                vehicle.Images
                    .Where(image => !image.IsDeleted)
                    .OrderByDescending(image => image.IsPrimary)
                    .ThenBy(image => image.CreatedDate)
                    .Select(image => new VehicleImageListResponse(
                        image.Id,
                        image.Url,
                        image.PublicId,
                        image.IsPrimary,
                        image.CreatedDate))
                    .ToList(),
                vehicle.FeatureAssignments
                    .Where(feature => !feature.IsDeleted
                                      && !feature.VehicleFeature.IsDeleted)
                    .OrderBy(feature => feature.VehicleFeature.Category)
                    .ThenBy(feature => feature.VehicleFeature.Name)
                    .Select(feature => new VehicleFeatureResponse(
                        feature.VehicleFeature.Id,
                        feature.VehicleFeature.Name,
                        feature.VehicleFeature.Category,
                        feature.VehicleFeature.IsActive))
                    .ToList(),
                vehicle.CreatedDate,
                vehicle.ModifiedDate))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Vehicle?> GetByIdAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Vehicles
            .Include(x => x.Rates)
            .Include(x => x.UnavailableDates)
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<Vehicle?> GetByIdWithImagesAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Vehicles
            .AsTracking()
            .Include(x => x.Rates)
            .Include(x => x.Images)
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<Vehicle?> GetByIdWithFeaturesAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Vehicles
            .AsTracking()
            .Include(x => x.FeatureAssignments)
            .ThenInclude(x => x.VehicleFeature)
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<bool> PlateNumberExistsAsync(
        Guid tenantId,
        string plateNumber,
        Guid? excludedVehicleId = null,
        CancellationToken cancellationToken = default)
    {
        string normalizedPlateNumber = NormalizePlateNumber(plateNumber);

        return await _context.Vehicles.AnyAsync(
            x => x.TenantId == tenantId
                 && x.PlateNumber == normalizedPlateNumber
                 && !x.IsDeleted
                 && (!excludedVehicleId.HasValue || x.Id != excludedVehicleId.Value),
            cancellationToken);
    }

    public async Task<bool> VinExistsAsync(
        Guid tenantId,
        string? vin,
        Guid? excludedVehicleId = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(vin))
            return false;

        string normalizedVin = vin.Trim().ToUpperInvariant();

        return await _context.Vehicles.AnyAsync(
            x => x.TenantId == tenantId
                 && x.Vin == normalizedVin
                 && !x.IsDeleted
                 && (!excludedVehicleId.HasValue || x.Id != excludedVehicleId.Value),
            cancellationToken);
    }

    private static string NormalizePlateNumber(string plateNumber)
    {
        return plateNumber.Trim().ToUpperInvariant().Replace("-", string.Empty).Replace(" ", string.Empty);
    }

    private sealed record ActiveRentalProjection(
        Guid VehicleId,
        Guid ReservationId,
        Guid CustomerId,
        string CustomerName,
        DateOnly StartDate,
        DateOnly EndDate);
}
