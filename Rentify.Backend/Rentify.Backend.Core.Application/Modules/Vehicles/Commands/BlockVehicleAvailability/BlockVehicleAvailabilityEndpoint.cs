using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.BlockVehicleAvailability;

public static class BlockVehicleAvailabilityEndpoint
{
    public static IEndpointRouteBuilder MapBlockVehicleAvailabilityEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/vehicles/{vehicleId:guid}/unavailable-dates", async(
            Guid vehicleId,
            BlockVehicleAvailabilityRequest request,
            ISender sender) =>
        {
            var command = new BlockVehicleAvailabilityCommand(
                vehicleId,
                request.StartDate,
                request.EndDate,
                request.Reason,
                request.CreatedBy);

            var response = await sender.Send(command);

            return Results.Created($"/api/v1/vehicles/{vehicleId}/unavailable-dates", response);
        }).WithTags("Vehicles");

        return app;
    }
}
