using MediatR;
using Rentify.Backend.Core.Application.Modules.Subscriptions.Dtos;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Subscriptions.Queries.GetCurrentSubscription
{
    public record GetCurrentSubscriptionQuery(Guid TenantId) : IRequest<ResultReponse<SubscriptionResponse>>;
}
