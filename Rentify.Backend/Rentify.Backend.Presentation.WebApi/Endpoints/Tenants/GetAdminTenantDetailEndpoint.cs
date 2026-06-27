using MediatR;
using Rentify.Backend.Core.Application.Modules.Tenants.Queries.GetAdminTenantDetail;

namespace Rentify.Backend.Presentation.WebApi.Endpoints.Tenants;

public static class GetAdminTenantDetailEndpoint
{
    public static IEndpointRouteBuilder MapGetAdminTenantDetailEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/{tenantId:guid}", async (
            Guid tenantId,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new GetAdminTenantDetailQuery(tenantId), cancellationToken);
            return Results.Ok(response);
        })
        .WithName("GetAdminTenantDetail")
        .WithSummary("Gets the full tenant detail for Super Admin.");

        return app;
    }
}
