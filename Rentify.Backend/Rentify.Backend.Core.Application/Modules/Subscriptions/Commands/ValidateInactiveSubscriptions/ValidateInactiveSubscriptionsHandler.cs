using MediatR;
using Rentify.Backend.Core.Application.Modules.Subscriptions.Contracts.Services;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Subscriptions.Commands.ValidateInactiveSubscriptions
{
    public class ValidateInactiveSubscriptionsHandler : IRequestHandler<ValidateInactiveSubscriptionsCommand, ResultReponse<ValidateInactiveSubscriptionsResponse>>
    {
        private readonly ISubscriptionService _subscriptionService;

        public ValidateInactiveSubscriptionsHandler(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        public async Task<ResultReponse<ValidateInactiveSubscriptionsResponse>> Handle(ValidateInactiveSubscriptionsCommand request, CancellationToken cancellationToken)
        {
            var response = await _subscriptionService.ValidateInactiveSubscriptionsAsync(cancellationToken);

            return ResultReponse<ValidateInactiveSubscriptionsResponse>.Success(response);
        }
    }
}
