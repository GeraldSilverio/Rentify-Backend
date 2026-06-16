using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetBrands;

public static class GetBrandsEndpoint
{
    public static IEndpointRouteBuilder MapGetBrandsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/vehicle-brands", async (ISender sender) =>
        {
            var response = await sender.Send(new GetBrandsQuery());

            return Results.Ok(response);
        })
        .WithTags("Vehicles");

        return app;
    }
}
