using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rentify.Backend.Core.Application.Modules.Shared.Context;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicleImages;

public static class GetVehicleImagesEndpoint
{
    public static IEndpointRouteBuilder MapGetVehicleImagesEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/vehicles/{vehicleId:guid}/images", async (
            Guid vehicleId,
            ICurrentTenantService currentTenantService,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new GetVehicleImagesQuery(currentTenantService.GetTenantId(), vehicleId),
                cancellationToken);

            return Results.Ok(response);
        })
        .WithTags("Vehicles");

        return app;
    }
}
