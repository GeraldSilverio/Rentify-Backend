using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UploadVehicleImage;

public static class UploadVehicleImageEndpoint
{
    public static IEndpointRouteBuilder MapUploadVehicleImageEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/tenants/{tenantId:guid}/vehicles/{vehicleId:guid}/images", async (
            Guid tenantId,
            Guid vehicleId,
            IFormFile image,
            [FromForm] bool isPrimary,
            [FromForm] string createdBy,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new UploadVehicleImageCommand(tenantId, vehicleId, image, isPrimary, createdBy),
                cancellationToken);

            return Results.Created(
                $"/api/v1/tenants/{tenantId}/vehicles/{vehicleId}/images/{response.Value}",
                response);
        })
        .DisableAntiforgery()
        .WithTags("Vehicles");

        return app;
    }
}
