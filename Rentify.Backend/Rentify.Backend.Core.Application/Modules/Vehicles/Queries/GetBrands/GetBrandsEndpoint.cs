using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rentify.Backend.Core.Application.Modules.Shared.Constants;

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

        app.MapGet("/api/v1/admin/vehicle-brands", async (ISender sender) =>
        {
            var response = await sender.Send(new GetBrandsQuery(false));

            return Results.Ok(response);
        })
        .RequireAuthorization(options => options.RequireRole(ApplicationRoles.SuperAdmin))
        .WithTags("Admin Vehicle Brands");

        return app;
    }
}
