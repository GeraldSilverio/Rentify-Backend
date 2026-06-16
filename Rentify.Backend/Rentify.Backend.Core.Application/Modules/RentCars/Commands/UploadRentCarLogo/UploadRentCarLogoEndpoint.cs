using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.RentCars.Commands.UploadRentCarLogo;

public static class UploadRentCarLogoEndpoint
{
    public static IEndpointRouteBuilder MapUploadRentCarLogoEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/rent-car/{rentCarId:guid}/logo", async(
            Guid rentCarId,
            IFormFile logo,
            string modifiedBy,
            ISender sender) =>
        {
            var command = new UploadRentCarLogoCommand(rentCarId, logo, modifiedBy);
            var response = await sender.Send(command);

            return Results.Ok(response);
        })
        .DisableAntiforgery()
        .WithTags("Rent Car");

        return app;
    }
}
