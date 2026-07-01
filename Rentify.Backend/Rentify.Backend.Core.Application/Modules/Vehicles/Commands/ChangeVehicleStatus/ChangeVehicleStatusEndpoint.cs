using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rentify.Backend.Core.Application.Modules.Shared.Context;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.ChangeVehicleStatus;

public static class ChangeVehicleStatusEndpoint
{
    public static IEndpointRouteBuilder MapChangeVehicleStatusEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/vehicles/{vehicleId:guid}/status", async(
            Guid vehicleId,
            ChangeVehicleStatusRequest request,
            ICurrentTenantService currentTenantService,
            ICurrentUserService currentUserService,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new ChangeVehicleStatusCommand(
                    currentTenantService.GetTenantId(),
                    vehicleId,
                    request.Status,
                    currentUserService.ModifiedBy),
                cancellationToken);

            return Results.Ok(response);
        }).WithTags("Vehicles");

        return app;
    }
}
