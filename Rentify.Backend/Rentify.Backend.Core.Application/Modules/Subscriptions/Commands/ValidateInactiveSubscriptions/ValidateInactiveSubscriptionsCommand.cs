using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Subscriptions.Contracts.Services;

namespace Rentify.Backend.Core.Application.Modules.Subscriptions.Commands.ValidateInactiveSubscriptions
{
    public record ValidateInactiveSubscriptionsCommand() : IRequest<ResultReponse<ValidateInactiveSubscriptionsResponse>>;
}
