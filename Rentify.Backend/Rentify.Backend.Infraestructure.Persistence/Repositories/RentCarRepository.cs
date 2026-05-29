using Rentify.Backend.Core.Application.Modules.RentCars.Contracts.Repositories;
using Rentify.Backend.Core.Domain.Entities;
using Rentify.Backend.Infraestructure.Persistence.Context;

namespace Rentify.Backend.Infrastructure.Persistence.Repositories;

public sealed class RentCarRepository
    : IRentCarRepository
{
    private readonly RentifyContext _context;

    public RentCarRepository(RentifyContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        RentCar rentCar,
        CancellationToken cancellationToken = default)
    {
        await _context.RentCars.AddAsync(
            rentCar,
            cancellationToken);
    }
}