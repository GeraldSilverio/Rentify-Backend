using Rentify.Backend.Core.Application.Modules.Tenants.Dtos;
using Rentify.Backend.Core.Domain.Entities.Core;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Services;

public interface ICurrentSubscriptionService
{
    Task<Subscription> GetCurrentSubscriptionAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<TenantSubscriptionResponse> GetCurrentSubscriptionResponseAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);
}
