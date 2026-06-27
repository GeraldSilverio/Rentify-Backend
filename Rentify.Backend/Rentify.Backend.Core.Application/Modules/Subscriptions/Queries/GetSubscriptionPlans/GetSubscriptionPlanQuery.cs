using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Subscriptions.Queries.GetSubscriptionPlans
{
    public record GetSubscriptionPlanQuery() : IRequest<ResultReponse<IEnumerable<GetSubscriptionPlanResponse>>>;
}
