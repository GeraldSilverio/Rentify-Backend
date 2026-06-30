using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.CreateVehicle;

public static class CreateVehicleEndpoint
{
    public static IEndpointRouteBuilder MapCreateVehicleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/tenants/{tenantId:guid}/vehicles", async (
            Guid tenantId,
            CreateVehicleRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateVehicleCommand(
                tenantId,
                request.RentCarId,
                request.VehicleBrandId,
                request.VehicleModelId,
                request.VehicleTypeId,
                request.Year,
                request.PlateNumber,
                request.Vin,
                request.Color,
                request.DailyRate,
                request.CurrentMileage,
                request.CreatedBy);

            var response = await sender.Send(command, cancellationToken);

            return Results.Created($"/api/v1/tenants/{tenantId}/vehicles/{response.Value}", response);
        })
        .WithTags("Vehicles");

        return app;
    }
}
