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
            CreateVehicleCommand command,
            ISender sender) =>
        {
            var response = await sender.Send(command);

            return Results.Created("/api/v1/vehicles", response);
        }).WithTags("Vehicles");

        return app;
    }
}
