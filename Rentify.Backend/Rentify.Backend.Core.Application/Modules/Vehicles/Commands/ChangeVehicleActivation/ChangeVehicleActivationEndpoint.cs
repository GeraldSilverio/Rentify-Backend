using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rentify.Backend.Core.Application.Modules.Shared.Context;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.ChangeVehicleActivation;

public static class ChangeVehicleActivationEndpoint
{
    public static IEndpointRouteBuilder MapChangeVehicleActivationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/vehicles/{vehicleId:guid}/activate", async (
            Guid vehicleId,
            ICurrentTenantService currentTenantService,
            ICurrentUserService currentUserService,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new ChangeVehicleActivationCommand(
                    currentTenantService.GetTenantId(),
                    vehicleId,
                    true,
                    currentUserService.ModifiedBy),
                cancellationToken);

            return Results.Ok(response);
        })
        .WithTags("Vehicles");

        app.MapPut("/api/v1/vehicles/{vehicleId:guid}/deactivate", async (
            Guid vehicleId,
            ICurrentTenantService currentTenantService,
            ICurrentUserService currentUserService,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new ChangeVehicleActivationCommand(
                    currentTenantService.GetTenantId(),
                    vehicleId,
                    false,
                    currentUserService.ModifiedBy),
                cancellationToken);

            return Results.Ok(response);
        })
        .WithTags("Vehicles");

        return app;
    }
}
