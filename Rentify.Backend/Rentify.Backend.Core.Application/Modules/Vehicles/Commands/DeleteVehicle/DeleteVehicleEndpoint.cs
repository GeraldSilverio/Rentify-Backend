using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rentify.Backend.Core.Application.Modules.Shared.Context;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.DeleteVehicle;

public static class DeleteVehicleEndpoint
{
    public static IEndpointRouteBuilder MapDeleteVehicleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/v1/vehicles/{vehicleId:guid}", async (
            Guid vehicleId,
            ICurrentTenantService currentTenantService,
            ICurrentUserService currentUserService,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new DeleteVehicleCommand(
                    currentTenantService.GetTenantId(),
                    vehicleId,
                    currentUserService.ModifiedBy),
                cancellationToken);

            return Results.Ok(response);
        })
        .WithTags("Vehicles");

        return app;
    }
}
