using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Domain.Entities;
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

    public async Task<Vehicle?> GetByIdAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Vehicles
            .Include(x => x.UnavailableDates)
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<Vehicle?> GetByIdWithImagesAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Vehicles
            .Include(x => x.Images)
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<bool> VehicleModelExistsAsync(Guid vehicleModelId, CancellationToken cancellationToken = default)
    {
        return await _context.VehicleModels.AnyAsync(x => x.Id == vehicleModelId && !x.IsDeleted, cancellationToken);
    }

    public async Task<bool> VehicleTypeExistsAsync(
        Guid tenantId,
        Guid vehicleTypeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.VehicleTypes.AnyAsync(
            x => x.TenantId == tenantId && x.Id == vehicleTypeId && !x.IsDeleted,
            cancellationToken);
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
}
