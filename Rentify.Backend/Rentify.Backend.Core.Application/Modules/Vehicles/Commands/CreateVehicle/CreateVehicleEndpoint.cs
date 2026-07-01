using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rentify.Backend.Core.Application.Modules.Shared.Context;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.CreateVehicle;

public static class CreateVehicleEndpoint
{
    public static IEndpointRouteBuilder MapCreateVehicleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/vehicles", async (
            CreateVehicleRequest request,
            ICurrentTenantService currentTenantService,
            ICurrentUserService currentUserService,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateVehicleCommand(
                currentTenantService.GetTenantId(),
                request.VehicleBrandId,
                request.VehicleModelId,
                request.VehicleTypeId,
                request.Year,
                request.PlateNumber,
                request.Vin,
                request.Color,
                request.CurrentMileage,
                request.Rates,
                request.FeatureIds ?? Array.Empty<Guid>(),
                currentUserService.ModifiedBy);

            var response = await sender.Send(command, cancellationToken);

            return Results.Created($"/api/v1/vehicles/{response.Value?.Id}", response);
        })
        .WithTags("Vehicles");

        return app;
    }
}
