using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.RentCars.Commands.UpdateRentCar;

public static class UpdateRentCarEndpoint
{
    public static IEndpointRouteBuilder MapUpdateRentCarEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/rent-car/{id:guid}", async(
            Guid id,
            UpdateRentCarRequest request,
            ISender sender) =>
        {
            var command = new UpdateRentCarCommand(
                id,
                request.Name,
                request.Description,
                request.AddressInformation,
                request.ContactInformation,
                request.LogoUrl,
                request.ModifiedBy);

            var response = await sender.Send(command);

            return Results.Ok(response);
        }).WithTags("Rent Car");

        return app;
    }
}
