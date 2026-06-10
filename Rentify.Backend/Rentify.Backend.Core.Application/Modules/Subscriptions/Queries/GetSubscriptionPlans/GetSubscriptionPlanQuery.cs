using MediatR;

namespace Rentify.Backend.Core.Application.Modules.Subscriptions.Queries.GetSubscriptionPlans
{
    public record GetSubscriptionPlanQuery() : IRequest<GetSubscriptionPlanResponse>;
}
