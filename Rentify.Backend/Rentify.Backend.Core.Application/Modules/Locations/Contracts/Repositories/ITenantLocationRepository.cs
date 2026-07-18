using Rentify.Backend.Core.Application.Modules.Locations.Dtos;
using Rentify.Backend.Core.Application.Modules.Locations.Queries.GetTenantLocations;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Domain.Entities.Locations;

namespace Rentify.Backend.Core.Application.Modules.Locations.Contracts.Repositories;

public interface ITenantLocationRepository
{
    Task AddAsync(TenantLocation tenantLocation, CancellationToken cancellationToken = default);
    Task<TenantLocation?> GetByIdAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default);
    Task<TenantLocationResponse?> GetDetailsAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default);
    Task<PaginatedResponse<TenantLocationListItemResponse>> GetPagedAsync(GetTenantLocationsQuery query, CancellationToken cancellationToken = default);
    Task<bool> ExistsByLocationAsync(Guid tenantId, Guid locationId, Guid? excludeId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByDisplayNameAsync(Guid tenantId, string displayName, Guid? excludeId, CancellationToken cancellationToken = default);
}
