using MediatR;
using Rentify.Backend.Core.Application.Modules.Tenants.Queries.GetAdminTenantSubscription;

namespace Rentify.Backend.Presentation.WebApi.Endpoints.Tenants;

public static class GetAdminTenantSubscriptionEndpoint
{
    public static IEndpointRouteBuilder MapGetAdminTenantSubscriptionEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/{tenantId:guid}/subscription", async (
            Guid tenantId,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new GetAdminTenantSubscriptionQuery(tenantId), cancellationToken);
            return Results.Ok(response);
        })
        .WithName("GetAdminTenantSubscription")
        .WithSummary("Gets the current subscription for a tenant as Super Admin.");

        return app;
    }
}
