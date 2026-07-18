using Rentify.Backend.Core.Application.Modules.Locations.Dtos;
using Rentify.Backend.Core.Application.Modules.Locations.Queries.GetAdminLocations;
using Rentify.Backend.Core.Application.Modules.Locations.Queries.GetAvailableLocations;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Domain.Entities.Locations;

namespace Rentify.Backend.Core.Application.Modules.Locations.Contracts.Repositories;

public interface ILocationRepository
{
    Task AddAsync(Location location, CancellationToken cancellationToken = default);
    Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Location?> GetActiveByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<LocationResponse?> GetDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PaginatedResponse<LocationListItemResponse>> GetPagedAsync(GetAdminLocationsQuery query, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AvailableLocationResponse>> GetAvailableAsync(GetAvailableLocationsQuery query, CancellationToken cancellationToken = default);
    Task<bool> ExistsDuplicateAsync(string name, string? city, string? province, string? country, Guid? excludeId, CancellationToken cancellationToken = default);
}
