using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.DeleteVehicleImage;

public static class DeleteVehicleImageEndpoint
{
    public static IEndpointRouteBuilder MapDeleteVehicleImageEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/v1/vehicles/{vehicleId:guid}/images/{imageId:guid}", async(
            Guid vehicleId,
            Guid imageId,
            string modifiedBy,
            ISender sender) =>
        {
            var response = await sender.Send(new DeleteVehicleImageCommand(vehicleId, imageId, modifiedBy));

            return Results.Ok(response);
        }).WithTags("Vehicles");

        return app;
    }
}
