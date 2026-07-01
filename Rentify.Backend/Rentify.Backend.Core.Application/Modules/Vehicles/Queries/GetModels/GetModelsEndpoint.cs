using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rentify.Backend.Core.Application.Modules.Shared.Constants;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetModels;

public static class GetModelsEndpoint
{
    public static IEndpointRouteBuilder MapGetModelsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/vehicle-brands/{vehicleBrandId:guid}/models", async (Guid vehicleBrandId, ISender sender) =>
        {
            var response = await sender.Send(new GetModelsQuery(vehicleBrandId));

            return response.IsSuccess
                ? Results.Ok(response)
                : Results.NotFound(response);
        })
        .WithTags("Vehicles");

        app.MapGet("/api/v1/admin/vehicle-models", async (ISender sender) =>
        {
            var response = await sender.Send(new GetModelsQuery(null, false));

            return Results.Ok(response);
        })
        .RequireAuthorization(options => options.RequireRole(ApplicationRoles.SuperAdmin))
        .WithTags("Admin Vehicle Models");

        return app;
    }
}
