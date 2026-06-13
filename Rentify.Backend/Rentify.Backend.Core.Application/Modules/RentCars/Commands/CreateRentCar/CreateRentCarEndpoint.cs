using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.RentCars.Commands.CreateRentCar;

public static class CreateRentCarEndpoint
{
    public static IEndpointRouteBuilder MapCreateRentCarEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/rent-car/create", async(
            CreateRentCarCommand command, 
            ISender sender) =>
        {
            var response = await sender.Send(command);
              
            return Results.Created("/api/v1/rent-car/create", response);
        }).WithTags("Rent Car");
        return app;
    }
    
}