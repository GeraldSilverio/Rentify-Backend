using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rentify.Backend.Core.Application.Modules.Shared.Context;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UpdateVehicle;

public static class UpdateVehicleEndpoint
{
    public static IEndpointRouteBuilder MapUpdateVehicleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/vehicles/{vehicleId:guid}", async (
            Guid vehicleId,
            UpdateVehicleRequest request,
            ICurrentTenantService currentTenantService,
            ICurrentUserService currentUserService,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new UpdateVehicleCommand(
                currentTenantService.GetTenantId(),
                vehicleId,
                request.VehicleBrandId,
                request.VehicleModelId,
                request.VehicleTypeId,
                request.Year,
                request.PlateNumber,
                request.Vin,
                request.Color,
                request.CurrentMileage,
                currentUserService.ModifiedBy), cancellationToken);

            return Results.Ok(response);
        })
        .WithTags("Vehicles");

        return app;
    }
}
