using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Domain.Entities;
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

    public async Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Vehicles
            .Include(x => x.Images)
            .Include(x => x.UnavailableDates)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<bool> PlateNumberExistsAsync(string plateNumber, CancellationToken cancellationToken = default)
    {
        string normalizedPlateNumber = plateNumber.Trim().ToUpperInvariant().Replace("-", string.Empty).Replace(" ", string.Empty);

        return await _context.Vehicles.AnyAsync(
            x => x.PlateNumber == normalizedPlateNumber && !x.IsDeleted,
            cancellationToken);
    }

    public async Task<bool> VinExistsAsync(string vin, CancellationToken cancellationToken = default)
    {
        string normalizedVin = vin.Trim().ToUpperInvariant();

        return await _context.Vehicles.AnyAsync(
            x => x.Vin == normalizedVin && !x.IsDeleted,
            cancellationToken);
    }
}
