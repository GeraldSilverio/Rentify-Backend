using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Context;
using Rentify.Backend.Core.Application.Modules.Tenants.Commands.ExtendTenantTrial;

namespace Rentify.Backend.Presentation.WebApi.Endpoints.Tenants;

public static class ExtendTenantTrialEndpoint
{
    public static IEndpointRouteBuilder MapExtendTenantTrialEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/{tenantId:guid}/subscription/extend-trial", async (
            Guid tenantId,
            ExtendTenantTrialRequest request,
            ICurrentUserService currentUserService,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new ExtendTenantTrialCommand(
                    tenantId,
                    request.DaysToAdd,
                    currentUserService.GetUserId()),
                cancellationToken);

            return Results.Ok(response);
        })
        .WithName("ExtendTenantTrial")
        .WithSummary("Extends the current subscription trial for a tenant as Super Admin.");

        return app;
    }
}
