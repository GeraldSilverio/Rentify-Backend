using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.CreateVehicle;

public static class CreateVehicleEndpoint
{
    public static IEndpointRouteBuilder MapCreateVehicleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/vehicles", async(
            Guid rentCarId,
            Guid modelId,
            int year,
            string plateNumber,
            string vin,
            string color,
            decimal dailyRate,
            IFormFile image,
            string createdBy,
            ISender sender) =>
        {
            var command = new CreateVehicleCommand(
                rentCarId,
                modelId,
                year,
                plateNumber,
                vin,
                color,
                dailyRate,
                image,
                createdBy);

            var response = await sender.Send(command);

            return Results.Created("/api/v1/vehicles", response);
        })
        .DisableAntiforgery()
        .WithTags("Vehicles");

        return app;
    }
}
