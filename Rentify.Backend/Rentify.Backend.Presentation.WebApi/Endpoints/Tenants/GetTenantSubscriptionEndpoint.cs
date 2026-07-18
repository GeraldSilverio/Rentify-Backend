using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Context;
using Rentify.Backend.Core.Application.Modules.Tenants.Queries.GetTenantSubscription;

namespace Rentify.Backend.Presentation.WebApi.Endpoints.Tenants;

public static class GetTenantSubscriptionEndpoint
{
    public static IEndpointRouteBuilder MapGetTenantSubscriptionEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/subscription", async (
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new GetTenantSubscriptionQuery(currentRequestContext.TenantId),
                cancellationToken);

            return Results.Ok(response);
        })
        .WithName("GetTenantSubscription")
        .WithSummary("Gets the current subscription for the authenticated tenant.");

        return app;
    }
}
