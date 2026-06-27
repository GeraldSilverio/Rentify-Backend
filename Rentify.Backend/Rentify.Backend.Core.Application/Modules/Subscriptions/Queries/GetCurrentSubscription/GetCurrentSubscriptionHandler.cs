using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Subscriptions.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Subscriptions.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;

namespace Rentify.Backend.Core.Application.Modules.Subscriptions.Queries.GetCurrentSubscription
{
    public class GetCurrentSubscriptionHandler : IRequestHandler<GetCurrentSubscriptionQuery, ResultReponse<SubscriptionResponse>>
    {
        private readonly ISubscriptionService _subscriptionService;

        public GetCurrentSubscriptionHandler(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        public async Task<ResultReponse<SubscriptionResponse>> Handle(GetCurrentSubscriptionQuery request, CancellationToken cancellationToken)
        {
            var subscription = await _subscriptionService.GetCurrentSubscriptionAsync(request.TenantId, cancellationToken);

            if (subscription == null)
            {
                throw new ApiException("Subscription not found.", StatusCodes.Status404NotFound);
            }

            var response = new SubscriptionResponse(
                subscription.Id,
                subscription.TenantId,
                subscription.SubscriptionPlanId,
                subscription.SubscriptionPlan.Name,
                subscription.Status,
                subscription.StartsAt,
                subscription.ExpiresAt,
                subscription.TrialEndsAt,
                subscription.IsTrial,
                subscription.AutoRenew);

            return ResultReponse<SubscriptionResponse>.Success(response);
        }
    }
}
