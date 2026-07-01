using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rentify.Backend.Core.Application.Modules.Shared.Context;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicleDetail;

public static class GetVehicleDetailEndpoint
{
    public static IEndpointRouteBuilder MapGetVehicleDetailEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/vehicles/{vehicleId:guid}", async (
            Guid vehicleId,
            ICurrentTenantService currentTenantService,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new GetVehicleDetailQuery(currentTenantService.GetTenantId(), vehicleId),
                cancellationToken);

            return Results.Ok(response);
        })
        .WithName("GetVehicleDetail")
        .WithTags("Vehicles")
        .WithSummary("Gets a vehicle detail for the authenticated tenant.");

        return app;
    }
}
