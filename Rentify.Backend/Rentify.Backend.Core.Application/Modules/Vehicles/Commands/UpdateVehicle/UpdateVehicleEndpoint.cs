using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UpdateVehicle;

public static class UpdateVehicleEndpoint
{
    public static IEndpointRouteBuilder MapUpdateVehicleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/tenants/{tenantId:guid}/vehicles/{vehicleId:guid}", async (
            Guid tenantId,
            Guid vehicleId,
            UpdateVehicleRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new UpdateVehicleCommand(
                tenantId,
                vehicleId,
                request.VehicleBrandId,
                request.VehicleModelId,
                request.VehicleTypeId,
                request.Year,
                request.PlateNumber,
                request.Vin,
                request.Color,
                request.DailyRate,
                request.CurrentMileage,
                request.ModifiedBy), cancellationToken);

            return Results.Ok(response);
        })
        .WithTags("Vehicles");

        return app;
    }
}
