using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Context;
using Rentify.Backend.Core.Application.Modules.Tenants.Queries.GetTenantUsage;

namespace Rentify.Backend.Presentation.WebApi.Endpoints.Tenants;

public static class GetTenantUsageEndpoint
{
    public static IEndpointRouteBuilder MapGetTenantUsageEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/usage", async (
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new GetTenantUsageQuery(currentRequestContext.TenantId),
                cancellationToken);

            return Results.Ok(response);
        })
        .WithName("GetTenantUsage")
        .WithSummary("Gets current plan usage for the authenticated tenant.");

        return app;
    }
}
