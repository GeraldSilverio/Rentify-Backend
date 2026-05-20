using Rentify.Backend.Core.Domain.Entities;

namespace Rentify.Backend.Core.Application.Contracts.Repositories;

public interface IRentCarRepository
{
    Task<RentCar?> GetByIdAsync(Guid id);
    Task<List<RentCar>> GetPagedAsync(int pageNumber, int pageSize);
    Task<int> GetTotalCountAsync();
    Task<RentCar> AddAsync(RentCar rentCar);
    Task UpdateAsync(RentCar rentCar);
}