using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rentify.Backend.Core.Application.Modules.Shared.Context;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.BlockVehicleAvailability;

public static class BlockVehicleAvailabilityEndpoint
{
    public static IEndpointRouteBuilder MapBlockVehicleAvailabilityEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/vehicles/{vehicleId:guid}/unavailable-dates", async(
            Guid vehicleId,
            BlockVehicleAvailabilityRequest request,
            ICurrentTenantService currentTenantService,
            ICurrentUserService currentUserService,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new BlockVehicleAvailabilityCommand(
                currentTenantService.GetTenantId(),
                vehicleId,
                request.StartDate,
                request.EndDate,
                request.Reason,
                currentUserService.ModifiedBy);

            var response = await sender.Send(command, cancellationToken);

            return Results.Created($"/api/v1/vehicles/{vehicleId}/unavailable-dates", response);
        }).WithTags("Vehicles");

        return app;
    }
}
