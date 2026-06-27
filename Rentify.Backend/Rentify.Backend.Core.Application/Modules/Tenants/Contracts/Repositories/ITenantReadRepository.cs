using Rentify.Backend.Core.Application.Modules.Tenants.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Tenants.Queries.GetAdminTenants;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Repositories;

public interface ITenantReadRepository
{
    Task<PaginatedResponse<AdminTenantListItemResponse>> GetAdminTenantsAsync(
        GetAdminTenantsQuery query,
        CancellationToken cancellationToken = default);

    Task<TenantProfileResponse?> GetProfileAsync(Guid tenantId, CancellationToken cancellationToken = default);

    Task<TenantSettingsResponse?> GetSettingsAsync(Guid tenantId, CancellationToken cancellationToken = default);

    Task<TenantPaymentPolicyResponse?> GetDefaultPaymentPolicyAsync(Guid tenantId, CancellationToken cancellationToken = default);

    Task<TenantSubscriptionResponse?> GetCurrentSubscriptionAsync(Guid tenantId, CancellationToken cancellationToken = default);

    Task<TenantUsageResponse> GetUsageAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
