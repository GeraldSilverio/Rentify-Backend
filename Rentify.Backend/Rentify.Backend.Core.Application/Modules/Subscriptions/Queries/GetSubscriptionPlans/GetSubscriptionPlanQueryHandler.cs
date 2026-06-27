using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Subscriptions.Contracts.Repositories;

namespace Rentify.Backend.Core.Application.Modules.Subscriptions.Queries.GetSubscriptionPlans
{
    public class GetSubscriptionPlanQueryHandler : IRequestHandler<GetSubscriptionPlanQuery, ResultReponse<IEnumerable<GetSubscriptionPlanResponse>>>
    {
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;

        public GetSubscriptionPlanQueryHandler(ISubscriptionPlanRepository subscriptionPlanRepository)
        {
            _subscriptionPlanRepository = subscriptionPlanRepository;
        }

        public async Task<ResultReponse<IEnumerable<GetSubscriptionPlanResponse>>> Handle(GetSubscriptionPlanQuery request, CancellationToken cancellationToken)
        {
            var subscriptionPlans = await _subscriptionPlanRepository.GetSubscriptionPlansAsync();   

            if(subscriptionPlans == null || !subscriptionPlans.Any()) throw new ApiException("No subscription plans found.",StatusCodes.Status404NotFound);

            return ResultReponse<IEnumerable<GetSubscriptionPlanResponse>>.Success(subscriptionPlans.ToList());
        }
    }
}
