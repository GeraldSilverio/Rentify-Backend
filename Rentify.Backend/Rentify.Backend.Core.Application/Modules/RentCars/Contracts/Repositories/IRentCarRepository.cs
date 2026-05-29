using Rentify.Backend.Core.Domain.Entities;

namespace Rentify.Backend.Core.Application.Modules.RentCars.Contracts.Repositories;

public interface IRentCarRepository
{
    Task AddAsync(
        RentCar rentCar,
        CancellationToken cancellationToken = default);
}