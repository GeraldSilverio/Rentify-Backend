using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rentify.Backend.Core.Application.Modules.Shared.Constants;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicleTypes;

public static class GetVehicleTypesEndpoint
{
    public static IEndpointRouteBuilder MapGetVehicleTypesEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/vehicle-types", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new GetVehicleTypesQuery(true), cancellationToken);
            return Results.Ok(response);
        })
        .WithTags("Vehicle Types");

        app.MapGet("/api/v1/admin/vehicle-types", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new GetVehicleTypesQuery(false), cancellationToken);
            return Results.Ok(response);
        })
        .RequireAuthorization(options => options.RequireRole(ApplicationRoles.SuperAdmin))
        .WithTags("Admin Vehicle Types");

        return app;
    }
}
