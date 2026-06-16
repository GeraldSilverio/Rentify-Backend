using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.ChangeVehicleStatus;

public static class ChangeVehicleStatusEndpoint
{
    public static IEndpointRouteBuilder MapChangeVehicleStatusEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/v1/tenants/{tenantId:guid}/vehicles/{vehicleId:guid}/status", async(
            Guid tenantId,
            Guid vehicleId,
            ChangeVehicleStatusRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new ChangeVehicleStatusCommand(tenantId, vehicleId, request.Status, request.ModifiedBy),
                cancellationToken);

            return Results.Ok(response);
        }).WithTags("Vehicles");

        return app;
    }
}
