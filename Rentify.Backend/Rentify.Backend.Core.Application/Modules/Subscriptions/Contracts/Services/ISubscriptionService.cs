using Rentify.Backend.Core.Application.Modules.Tenants.Commands.RegisterTenant;

namespace Rentify.Backend.Core.Application.Modules.Subscriptions.Contracts.Services;

public interface ISubscriptionService
{
    Task RegisterSubscriptionAsync(Guid planId, Guid tenantId,RegisterTenantCommand command, CancellationToken cancellationToken);
}