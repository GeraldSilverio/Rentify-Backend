using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rentify.Backend.Core.Application.Modules.Shared.Context;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetAssignedVehicleFeatures;

public static class GetAssignedVehicleFeaturesEndpoint
{
    public static IEndpointRouteBuilder MapGetAssignedVehicleFeaturesEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/vehicles/{vehicleId:guid}/features", async (
            Guid vehicleId,
            ICurrentTenantService currentTenantService,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new GetAssignedVehicleFeaturesQuery(currentTenantService.GetTenantId(), vehicleId),
                cancellationToken);

            return Results.Ok(response);
        })
        .WithTags("Vehicle Features");

        return app;
    }
}
