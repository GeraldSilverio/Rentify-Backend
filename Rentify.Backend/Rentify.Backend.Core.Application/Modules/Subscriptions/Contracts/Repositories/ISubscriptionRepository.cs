using Rentify.Backend.Core.Domain.Entities.Core;

namespace Rentify.Backend.Core.Application.Modules.Subscriptions.Contracts.Repositories;

public interface ISubscriptionRepository
{
    Task<SubscriptionPlan?> GetPlanByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<Guid> GetSubscriptionPlanIdByCodeAsync(string code, 
        CancellationToken cancellationToken = default);
    Task AddAsync(
        Subscription subscription,
        CancellationToken cancellationToken = default);
}