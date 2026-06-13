using Rentify.Backend.Core.Application.Modules.Subscriptions.Queries.GetSubscriptionPlans;

namespace Rentify.Backend.Core.Application.Modules.Subscriptions.Contracts.Repositories
{
    public interface ISubscriptionPlanRepository
    {
        Task<IEnumerable<GetSubscriptionPlanResponse>> GetSubscriptionPlansAsync();
    }
}
