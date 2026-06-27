using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Context;
using Rentify.Backend.Core.Application.Modules.Tenants.Queries.GetTenantUsage;

namespace Rentify.Backend.Presentation.WebApi.Endpoints.Tenants;

public static class GetTenantUsageEndpoint
{
    public static IEndpointRouteBuilder MapGetTenantUsageEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/usage", async (
            ICurrentTenantService currentTenantService,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new GetTenantUsageQuery(currentTenantService.GetTenantId()),
                cancellationToken);

            return Results.Ok(response);
        })
        .WithName("GetTenantUsage")
        .WithSummary("Gets current plan usage for the authenticated tenant.");

        return app;
    }
}
