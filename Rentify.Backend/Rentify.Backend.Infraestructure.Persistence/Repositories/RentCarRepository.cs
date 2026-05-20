using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Core.Application.Contracts.Repositories;
using Rentify.Backend.Core.Domain.Entities;
using Rentify.Backend.Infraestructure.Persistence.Context;

namespace Rentify.Backend.Infraestructure.Persistence.Repositories;

public class RentCarRepository : IRentCarRepository
{
    private readonly RentifyContext _context;

    public RentCarRepository(RentifyContext context)
    {
        _context = context;
    }

    public async Task<RentCar?> GetByIdAsync(Guid id)
    {
        return await _context.RentCars
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<RentCar>> GetPagedAsync(int pageNumber, int pageSize)
    {
        return await _context.RentCars
            .AsNoTracking()
            .Where(r => !r.IsDeleted)
            .OrderByDescending(r => r.CreatedDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _context.RentCars
            .AsNoTracking()
            .CountAsync(r => !r.IsDeleted);
    }

    public async Task<RentCar> AddAsync(RentCar rentCar)
    {
        await _context.RentCars.AddAsync(rentCar);
        await _context.SaveChangesAsync();
        return rentCar;
    }

    public async Task UpdateAsync(RentCar rentCar)
    {
        _context.RentCars.Update(rentCar);
        await _context.SaveChangesAsync();
    }
}
