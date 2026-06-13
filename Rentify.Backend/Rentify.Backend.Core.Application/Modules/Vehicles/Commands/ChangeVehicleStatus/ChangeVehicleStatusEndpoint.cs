using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.ChangeVehicleStatus;

public static class ChangeVehicleStatusEndpoint
{
    public static IEndpointRouteBuilder MapChangeVehicleStatusEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/v1/vehicles/{vehicleId:guid}/status", async(
            Guid vehicleId,
            ChangeVehicleStatusRequest request,
            ISender sender) =>
        {
            var response = await sender.Send(new ChangeVehicleStatusCommand(vehicleId, request.Status, request.ModifiedBy));

            return Results.Ok(response);
        }).WithTags("Vehicles");

        return app;
    }
}
