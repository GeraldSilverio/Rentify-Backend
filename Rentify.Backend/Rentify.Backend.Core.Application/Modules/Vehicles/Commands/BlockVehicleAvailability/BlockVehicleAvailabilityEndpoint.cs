using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.BlockVehicleAvailability;

public static class BlockVehicleAvailabilityEndpoint
{
    public static IEndpointRouteBuilder MapBlockVehicleAvailabilityEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/tenants/{tenantId:guid}/vehicles/{vehicleId:guid}/unavailable-dates", async(
            Guid tenantId,
            Guid vehicleId,
            BlockVehicleAvailabilityRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new BlockVehicleAvailabilityCommand(
                tenantId,
                vehicleId,
                request.StartDate,
                request.EndDate,
                request.Reason,
                request.CreatedBy);

            var response = await sender.Send(command, cancellationToken);

            return Results.Created($"/api/v1/tenants/{tenantId}/vehicles/{vehicleId}/unavailable-dates", response);
        }).WithTags("Vehicles");

        return app;
    }
}
