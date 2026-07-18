using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Context;
using Rentify.Backend.Core.Application.Modules.Tenants.Queries.GetTenantSettings;

namespace Rentify.Backend.Presentation.WebApi.Endpoints.Tenants;

public static class GetTenantSettingsEndpoint
{
    public static IEndpointRouteBuilder MapGetTenantSettingsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/settings", async (
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new GetTenantSettingsQuery(currentRequestContext.TenantId),
                cancellationToken);

            return Results.Ok(response);
        })
        .WithName("GetTenantSettings")
        .WithSummary("Gets settings for the authenticated tenant.");

        return app;
    }
}
