using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Context;
using Rentify.Backend.Core.Application.Modules.Tenants.Commands.UpdateTenantSettings;

namespace Rentify.Backend.Presentation.WebApi.Endpoints.Tenants;

public static class UpdateTenantSettingsEndpoint
{
    public static IEndpointRouteBuilder MapUpdateTenantSettingsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/settings", async (
            UpdateTenantSettingsRequest request,
            ICurrentTenantService currentTenantService,
            ICurrentUserService currentUserService,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new UpdateTenantSettingsCommand(
                    currentTenantService.GetTenantId(),
                    request.CurrencyCode,
                    request.TimeZone,
                    request.EnableReservations,
                    request.EnableDriverFleet,
                    request.EnableMaintenance,
                    request.EnableLateFees,
                    request.EnablePublicCatalog,
                    currentUserService.GetUserId()),
                cancellationToken);

            return Results.Ok(response);
        })
        .WithName("UpdateTenantSettings")
        .WithSummary("Updates settings for the authenticated tenant.");

        return app;
    }
}
