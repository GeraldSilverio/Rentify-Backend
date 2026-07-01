using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rentify.Backend.Core.Application.Modules.Shared.Context;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.SetPrimaryVehicleImage;

public static class SetPrimaryVehicleImageEndpoint
{
    public static IEndpointRouteBuilder MapSetPrimaryVehicleImageEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/vehicles/{vehicleId:guid}/images/{imageId:guid}/set-primary", async (
            Guid vehicleId,
            Guid imageId,
            ICurrentTenantService currentTenantService,
            ICurrentUserService currentUserService,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new SetPrimaryVehicleImageCommand(
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
