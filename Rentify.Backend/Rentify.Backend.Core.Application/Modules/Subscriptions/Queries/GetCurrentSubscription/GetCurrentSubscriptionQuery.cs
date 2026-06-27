using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Subscriptions.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Subscriptions.Queries.GetCurrentSubscription
{
    public record GetCurrentSubscriptionQuery(Guid TenantId) : IRequest<ResultReponse<SubscriptionResponse>>;
}
