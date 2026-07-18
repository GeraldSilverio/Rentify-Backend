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
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new UpdateTenantSettingsCommand(
                    currentRequestContext.TenantId,
                    request.CurrencyCode,
                    request.TimeZone,
                    request.EnableReservations,
                    request.EnableDriverFleet,
                    request.EnableMaintenance,
                    request.EnableLateFees,
                    request.EnablePublicCatalog,
                    currentRequestContext.ModifiedBy),
                cancellationToken);

            return Results.Ok(response);
        })
        .WithName("UpdateTenantSettings")
        .WithSummary("Updates settings for the authenticated tenant.");

        return app;
    }
}
