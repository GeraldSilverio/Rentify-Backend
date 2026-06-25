using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Core.Application.Common.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;
using Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicles;
using Rentify.Backend.Core.Domain.Entities.Vehicles;
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
                || EF.Functions.ILike(vehicle.Vin, searchPattern)
                || EF.Functions.ILike(vehicle.VehicleModel.Name, searchPattern)
                || EF.Functions.ILike(vehicle.VehicleModel.VehicleBrand.Name, searchPattern));
        }

        if (query.VehicleTypeId.HasValue)
            vehiclesQuery = vehiclesQuery.Where(vehicle => vehicle.VehicleTypeId == query.VehicleTypeId.Value);

        if (query.VehicleBrandId.HasValue)
            vehiclesQuery = vehiclesQuery.Where(vehicle => vehicle.VehicleModel.VehicleBrandId == query.VehicleBrandId.Value);

        if (query.VehicleModelId.HasValue)
            vehiclesQuery = vehiclesQuery.Where(vehicle => vehicle.VehicleModelId == query.VehicleModelId.Value);

        if (query.Status.HasValue)
            vehiclesQuery = vehiclesQuery.Where(vehicle => vehicle.Status == query.Status.Value);

        if (query.MinDailyRate.HasValue)
            vehiclesQuery = vehiclesQuery.Where(vehicle => vehicle.DailyRate >= query.MinDailyRate.Value);

        if (query.MaxDailyRate.HasValue)
            vehiclesQuery = vehiclesQuery.Where(vehicle => vehicle.DailyRate <= query.MaxDailyRate.Value);

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
                vehicle.RentCarId,
                vehicle.VehicleModelId,
                vehicle.VehicleModel.Name,
                vehicle.VehicleModel.VehicleBrandId,
                vehicle.VehicleModel.VehicleBrand.Name,
                vehicle.VehicleTypeId,
                vehicle.VehicleType.Name,
                vehicle.Year,
                vehicle.PlateNumber,
                vehicle.Vin,
                vehicle.Color,
                vehicle.DailyRate,
                vehicle.Status,
                vehicle.IsActive,
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
                vehicle.CreatedDate,
                vehicle.ModifiedDate))
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

    public async Task<Vehicle?> GetByIdAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Vehicles
            .Include(x => x.UnavailableDates)
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<Vehicle?> GetByIdWithImagesAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Vehicles
            .AsTracking()
            .Include(x => x.Images)
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
        string vin,
        Guid? excludedVehicleId = null,
        CancellationToken cancellationToken = default)
    {
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
