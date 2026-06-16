using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.SetPrimaryVehicleImage;

public static class SetPrimaryVehicleImageEndpoint
{
    public static IEndpointRouteBuilder MapSetPrimaryVehicleImageEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/v1/tenants/{tenantId:guid}/vehicles/{vehicleId:guid}/images/{imageId:guid}/primary", async (
            Guid tenantId,
            Guid vehicleId,
            Guid imageId,
            string modifiedBy,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new SetPrimaryVehicleImageCommand(tenantId, vehicleId, imageId, modifiedBy),
                cancellationToken);

            return Results.Ok(response);
        })
        .WithTags("Vehicles");

        return app;
    }
}
