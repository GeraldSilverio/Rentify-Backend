using MediatR;
using Rentify.Backend.Core.Application.Modules.Subscriptions.Contracts.Services;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Subscriptions.Commands.ValidateInactiveSubscriptions
{
    public record ValidateInactiveSubscriptionsCommand() : IRequest<ResultReponse<ValidateInactiveSubscriptionsResponse>>;
}
