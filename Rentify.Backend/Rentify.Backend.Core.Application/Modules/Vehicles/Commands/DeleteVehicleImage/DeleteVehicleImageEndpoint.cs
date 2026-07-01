using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rentify.Backend.Core.Application.Modules.Shared.Context;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.DeleteVehicleImage;

public static class DeleteVehicleImageEndpoint
{
    public static IEndpointRouteBuilder MapDeleteVehicleImageEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/v1/vehicles/{vehicleId:guid}/images/{imageId:guid}", async (
            Guid vehicleId,
            Guid imageId,
            ICurrentTenantService currentTenantService,
            ICurrentUserService currentUserService,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new DeleteVehicleImageCommand(
                    currentTenantService.GetTenantId(),
                    vehicleId,
                    imageId,
                    currentUserService.ModifiedBy),
                cancellationToken);

            return Results.Ok(response);
        })
        .WithTags("Vehicles");

        return app;
    }
}
