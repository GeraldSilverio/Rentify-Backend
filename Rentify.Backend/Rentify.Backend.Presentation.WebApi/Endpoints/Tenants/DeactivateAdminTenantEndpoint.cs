using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Context;
using Rentify.Backend.Core.Application.Modules.Tenants.Commands.DeactivateAdminTenant;

namespace Rentify.Backend.Presentation.WebApi.Endpoints.Tenants;

public static class DeactivateAdminTenantEndpoint
{
    public static IEndpointRouteBuilder MapDeactivateAdminTenantEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/{tenantId:guid}/deactivate", async (
            Guid tenantId,
            ICurrentUserService currentUserService,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new DeactivateAdminTenantCommand(tenantId, currentUserService.ModifiedBy),
                cancellationToken);

            return Results.Ok(response);
        })
        .WithName("DeactivateAdminTenant")
        .WithSummary("Deactivates a tenant as Super Admin.");

        return app;
    }
}
