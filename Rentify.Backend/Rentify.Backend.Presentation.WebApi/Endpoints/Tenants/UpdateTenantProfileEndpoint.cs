using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Context;
using Rentify.Backend.Core.Application.Modules.Tenants.Commands.UpdateTenantProfile;

namespace Rentify.Backend.Presentation.WebApi.Endpoints.Tenants;

public static class UpdateTenantProfileEndpoint
{
    public static IEndpointRouteBuilder MapUpdateTenantProfileEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/profile", async (
            UpdateTenantProfileRequest request,
            ICurrentTenantService currentTenantService,
            ICurrentUserService currentUserService,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new UpdateTenantProfileCommand(
                    currentTenantService.GetTenantId(),
                    request.Name,
                    request.LegalName,
                    request.Rnc,
                    currentUserService.ModifiedBy),
                cancellationToken);

            return Results.Ok(response);
        })
        .WithName("UpdateTenantProfile")
        .WithSummary("Updates the profile for the authenticated tenant.");

        return app;
    }
}
