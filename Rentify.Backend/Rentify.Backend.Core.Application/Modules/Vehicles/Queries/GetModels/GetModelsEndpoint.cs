using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetModels;

public static class GetModelsEndpoint
{
    public static IEndpointRouteBuilder MapGetModelsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/brands/{brandId:guid}/models", async (Guid brandId, ISender sender) =>
        {
            var response = await sender.Send(new GetModelsQuery(brandId));

            return response.IsSuccess
                ? Results.Ok(response)
                : Results.NotFound(response);
        })
        .WithTags("Vehicle Catalog");

        return app;
    }
}
