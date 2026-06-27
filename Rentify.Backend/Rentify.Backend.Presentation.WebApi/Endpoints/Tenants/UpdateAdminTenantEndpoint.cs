using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Context;
using Rentify.Backend.Core.Application.Modules.Tenants.Commands.UpdateAdminTenant;

namespace Rentify.Backend.Presentation.WebApi.Endpoints.Tenants;

public static class UpdateAdminTenantEndpoint
{
    public static IEndpointRouteBuilder MapUpdateAdminTenantEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/{tenantId:guid}", async (
            Guid tenantId,
            UpdateAdminTenantRequest request,
            ICurrentUserService currentUserService,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new UpdateAdminTenantCommand(
                    tenantId,
                    request.Name,
                    request.LegalName,
                    request.Rnc,
                    request.BusinessModel,
                    currentUserService.GetUserId()),
                cancellationToken);

            return Results.Ok(response);
        })
        .WithName("UpdateAdminTenant")
        .WithSummary("Updates a tenant as Super Admin.");

        return app;
    }
}
