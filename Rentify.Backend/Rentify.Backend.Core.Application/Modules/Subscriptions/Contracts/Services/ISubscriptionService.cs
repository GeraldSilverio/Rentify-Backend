using Rentify.Backend.Core.Application.Modules.Tenants.Commands.RegisterTenant;
using Rentify.Backend.Core.Domain.Entities.Core;

namespace Rentify.Backend.Core.Application.Modules.Subscriptions.Contracts.Services;

public interface ISubscriptionService
{
    Task<Subscription> RegisterSubscriptionAsync(Guid tenantId, RegisterTenantCommand command, CancellationToken cancellationToken);
    Task<Subscription?> GetCurrentSubscriptionAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<ValidateInactiveSubscriptionsResponse> ValidateInactiveSubscriptionsAsync(CancellationToken cancellationToken = default);
}
