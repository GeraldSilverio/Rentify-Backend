using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Subscriptions.Queries.GetCurrentSubscription
{
    public static class GetCurrentSubscriptionEndpoint
    {
        public static IEndpointRouteBuilder MapGetCurrentSubscription(this IEndpointRouteBuilder app)
        {
            app.MapGet("/tenants/{tenantId:guid}/current",
                async (Guid tenantId, ISender sender) =>
                {
                    var response = await sender.Send(new GetCurrentSubscriptionQuery(tenantId));

                    return Results.Ok(response);
                });

            return app;
        }
    }
}
