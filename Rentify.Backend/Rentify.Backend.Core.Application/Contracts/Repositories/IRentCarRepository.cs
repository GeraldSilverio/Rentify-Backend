using Rentify.Backend.Core.Domain.Entities;

namespace Rentify.Backend.Core.Application.Contracts.Repositories;

public interface IRentCarRepository
{
    Task<List<RentCar>> GetRentCarsAsync(int page, int pageSize);
    Task<RentCar> GetRentCarByIdAsync(int id);
    Task<RentCar> AddRentCarAsync(RentCar rentCar);
    Task<RentCar> UpdateRentCarAsync(RentCar rentCar);
    Task DeleteRentCarAsync(int id);
    Task DisableRentCarAsync(int id);
    Task EnableRentCarAsync(int id);
}