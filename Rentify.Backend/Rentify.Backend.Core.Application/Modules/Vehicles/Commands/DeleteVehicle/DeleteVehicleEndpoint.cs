using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.DeleteVehicle;

public static class DeleteVehicleEndpoint
{
    public static IEndpointRouteBuilder MapDeleteVehicleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/v1/tenants/{tenantId:guid}/vehicles/{vehicleId:guid}", async (
            Guid tenantId,
            Guid vehicleId,
            string modifiedBy,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new DeleteVehicleCommand(tenantId, vehicleId, modifiedBy), cancellationToken);

            return Results.Ok(response);
        })
        .WithTags("Vehicles");

        return app;
    }
}
