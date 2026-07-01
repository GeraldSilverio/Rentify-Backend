using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;
using Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicles;
using Rentify.Backend.Core.Domain.Entities;
using Rentify.Backend.Core.Domain.Entities.Vehicles;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;

public interface IVehicleRepository
{
    Task AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default);
    Task AddImagesAsync(IEnumerable<VehicleImage> images, CancellationToken cancellationToken = default);
    Task<PaginatedResponse<VehicleListItemResponse>> GetPagedAsync(GetVehiclesQuery query, CancellationToken cancellationToken = default);
    Task<VehicleDetailResponse?> GetDetailAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default);
    Task<Vehicle?> GetByIdAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default);
    Task<Vehicle?> GetByIdWithImagesAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default);
    Task<Vehicle?> GetByIdWithFeaturesAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default);
    Task<bool> PlateNumberExistsAsync(Guid tenantId, string plateNumber, Guid? excludedVehicleId = null, CancellationToken cancellationToken = default);
    Task<bool> VinExistsAsync(Guid tenantId, string? vin, Guid? excludedVehicleId = null, CancellationToken cancellationToken = default);
}
