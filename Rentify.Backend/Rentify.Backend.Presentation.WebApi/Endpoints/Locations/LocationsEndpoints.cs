using MediatR;
using Rentify.Backend.Core.Application.Modules.Locations.Queries.GetAvailableLocations;
using Rentify.Backend.Core.Application.Modules.Shared.Constants;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Presentation.WebApi.Endpoints.Locations;

public static class LocationsEndpoints
{
    public static IEndpointRouteBuilder MapLocationsEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app
            .MapGroup("/api/v1/locations")
            .WithTags("Locations")
            .RequireAuthorization(AuthorizationPolicies.RequiredRoles);

        group.MapGet("/available", async (
            string? search,
            LocationType? type,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new GetAvailableLocationsQuery(search, type), cancellationToken);
            return Results.Ok(response);
        });

        return app;
    }
}
