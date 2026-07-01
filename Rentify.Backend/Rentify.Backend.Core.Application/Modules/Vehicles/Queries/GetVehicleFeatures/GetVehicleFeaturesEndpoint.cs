using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rentify.Backend.Core.Application.Modules.Shared.Constants;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicleFeatures;

public static class GetVehicleFeaturesEndpoint
{
    public static IEndpointRouteBuilder MapGetVehicleFeaturesEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/vehicle-features", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new GetVehicleFeaturesQuery(true), cancellationToken);
            return Results.Ok(response);
        })
        .WithTags("Vehicle Features");

        app.MapGet("/api/v1/admin/vehicle-features", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new GetVehicleFeaturesQuery(false), cancellationToken);
            return Results.Ok(response);
        })
        .RequireAuthorization(options => options.RequireRole(ApplicationRoles.SuperAdmin))
        .WithTags("Admin Vehicle Features");

        return app;
    }
}
