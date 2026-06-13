using Rentify.Backend.Core.Domain.Entities.Core;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Repositories;

public interface ITenantRepository
{
    Task<bool> SlugExistsAsync(
        string slug,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Tenant tenant,
        CancellationToken cancellationToken = default);
}