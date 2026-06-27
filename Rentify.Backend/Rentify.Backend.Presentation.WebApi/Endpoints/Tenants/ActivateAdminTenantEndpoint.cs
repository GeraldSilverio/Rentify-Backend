using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Context;
using Rentify.Backend.Core.Application.Modules.Tenants.Commands.ActivateAdminTenant;

namespace Rentify.Backend.Presentation.WebApi.Endpoints.Tenants;

public static class ActivateAdminTenantEndpoint
{
    public static IEndpointRouteBuilder MapActivateAdminTenantEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/{tenantId:guid}/activate", async (
            Guid tenantId,
            ICurrentUserService currentUserService,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new ActivateAdminTenantCommand(tenantId, currentUserService.GetUserId()),
                cancellationToken);

            return Results.Ok(response);
        })
        .WithName("ActivateAdminTenant")
        .WithSummary("Activates a tenant as Super Admin.");

        return app;
    }
}
