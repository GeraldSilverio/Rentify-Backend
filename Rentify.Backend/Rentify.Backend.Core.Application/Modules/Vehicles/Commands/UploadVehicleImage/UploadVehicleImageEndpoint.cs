using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UploadVehicleImage;

public static class UploadVehicleImageEndpoint
{
    public static IEndpointRouteBuilder MapUploadVehicleImageEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/vehicles/{vehicleId:guid}/images", async(
            Guid vehicleId,
            IFormFile image,
            bool isPrimary,
            string createdBy,
            ISender sender) =>
        {
            var command = new UploadVehicleImageCommand(vehicleId, image, isPrimary, createdBy);
            var response = await sender.Send(command);

            return Results.Created($"/api/v1/vehicles/{vehicleId}/images", response);
        })
        .DisableAntiforgery()
        .WithTags("Vehicles");

        return app;
    }
}
