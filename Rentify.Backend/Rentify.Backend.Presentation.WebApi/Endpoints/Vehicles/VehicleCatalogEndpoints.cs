using MediatR;
using Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetBrands;
using Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetModels;
using Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicleFeatures;
using Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicleTypes;

namespace Rentify.Backend.Presentation.WebApi.Endpoints.Vehicles
{
    public static class VehicleCatalogEndpoints
    {
        public static IEndpointRouteBuilder MapVehicleCatalogEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/v1/vehicles-catalog")
                .WithTags("vehicles-catalog")
                .RequireRateLimiting("PublicCatalogPolicy");

            group.MapGet("/types", async (
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var response = await sender.Send(new GetVehicleTypesQuery(), cancellationToken);
                return Results.Ok(response);
            })
            .WithSummary("Get Vehicle Types");

            group.MapGet("/brands", async (
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var response = await sender.Send(new GetBrandsQuery(), cancellationToken);
                return Results.Ok(response);
            })
            .WithSummary("Get Vehicle Brands");

            group.MapGet("/{brandId:guid}/models", async (
                Guid brandId,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var response = await sender.Send(new GetModelsQuery(brandId), cancellationToken);
                return Results.Ok(response);
            })
            .WithSummary("Get Vehicle Models");

            group.MapGet("/features", async (
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var response = await sender.Send(new GetVehicleFeaturesQuery(), cancellationToken);
                return Results.Ok(response);
            });

            return app;
        }
    }
}
