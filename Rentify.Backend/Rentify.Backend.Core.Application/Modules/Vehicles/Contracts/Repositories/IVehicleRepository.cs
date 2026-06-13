using Rentify.Backend.Core.Domain.Entities;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;

public interface IVehicleRepository
{
    Task AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default);
    Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> PlateNumberExistsAsync(string plateNumber, CancellationToken cancellationToken = default);
    Task<bool> VinExistsAsync(string vin, CancellationToken cancellationToken = default);
}
