using Rentify.Backend.Core.Domain.Entities.Core;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Services;

public interface ITenantAccessService
{
    Task<Tenant> GetTenantAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<Tenant> GetActiveTenantAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task EnsureTenantIsActiveAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<bool> IsTenantActiveAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);
}
