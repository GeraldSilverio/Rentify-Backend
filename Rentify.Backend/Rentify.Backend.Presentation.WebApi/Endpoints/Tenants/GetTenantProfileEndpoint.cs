using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Context;
using Rentify.Backend.Core.Application.Modules.Tenants.Queries.GetTenantProfile;

namespace Rentify.Backend.Presentation.WebApi.Endpoints.Tenants;

public static class GetTenantProfileEndpoint
{
    public static IEndpointRouteBuilder MapGetTenantProfileEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/profile", async (
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new GetTenantProfileQuery(currentRequestContext.TenantId),
                cancellationToken);

            return Results.Ok(response);
        })
        .WithName("GetTenantProfile")
        .WithSummary("Gets the profile for the authenticated tenant.");

        return app;
    }
}
