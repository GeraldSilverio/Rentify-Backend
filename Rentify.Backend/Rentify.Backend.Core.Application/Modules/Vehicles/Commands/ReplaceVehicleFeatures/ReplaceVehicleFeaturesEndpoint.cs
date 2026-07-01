using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rentify.Backend.Core.Application.Modules.Shared.Context;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.ReplaceVehicleFeatures;

public static class ReplaceVehicleFeaturesEndpoint
{
    public static IEndpointRouteBuilder MapReplaceVehicleFeaturesEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/vehicles/{vehicleId:guid}/features", async (
            Guid vehicleId,
            ReplaceVehicleFeaturesRequest request,
            ICurrentTenantService currentTenantService,
            ICurrentUserService currentUserService,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new ReplaceVehicleFeaturesCommand(
                    currentTenantService.GetTenantId(),
                    vehicleId,
                    request.FeatureIds,
                    currentUserService.ModifiedBy),
                cancellationToken);

            return Results.Ok(response);
        })
        .WithTags("Vehicle Features");

        return app;
    }
}
