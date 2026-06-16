using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

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

        return app;
    }
}
