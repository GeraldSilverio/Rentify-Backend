using MediatR;
using Rentify.Backend.Core.Application.Modules.Locations.Commands.ActivateLocation;
using Rentify.Backend.Core.Application.Modules.Locations.Commands.CreateLocation;
using Rentify.Backend.Core.Application.Modules.Locations.Commands.DeactivateLocation;
using Rentify.Backend.Core.Application.Modules.Locations.Commands.DeleteLocation;
using Rentify.Backend.Core.Application.Modules.Locations.Commands.UpdateLocation;
using Rentify.Backend.Core.Application.Modules.Locations.Queries.GetAdminLocationById;
using Rentify.Backend.Core.Application.Modules.Locations.Queries.GetAdminLocations;
using Rentify.Backend.Core.Application.Modules.Shared.Constants;
using Rentify.Backend.Core.Application.Modules.Shared.Context;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Presentation.WebApi.Endpoints.Locations;

public static class AdminLocationsEndpoints
{
    public static IEndpointRouteBuilder MapAdminLocationsEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app
            .MapGroup("/api/v1/admin/locations")
            .WithTags("Admin Locations")
            .RequireAuthorization(options => options.RequireRole(ApplicationRoles.SuperAdmin));

        group.MapGet("/", async (
            string? search,
            LocationType? type,
            bool? isActive,
            ISender sender,
            CancellationToken cancellationToken,
            int pageNumber = 1,
            int pageSize = 10) =>
        {
            var response = await sender.Send(new GetAdminLocationsQuery(search, type, isActive, pageNumber, pageSize), cancellationToken);
            return Results.Ok(response);
        });

        group.MapGet("/{locationId:guid}", async (
            Guid locationId,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new GetAdminLocationByIdQuery(locationId), cancellationToken);
            return Results.Ok(response);
        });

        group.MapPost("/", async (
            CreateLocationRequest request,
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new CreateLocationCommand(
                request.Name,
                request.Type,
                request.City,
                request.Province,
                request.Country,
                request.Address,
                request.Notes,
                currentRequestContext.ModifiedBy), cancellationToken);

            return Results.Created($"/api/v1/admin/locations/{response.Value?.Id}", response);
        });

        group.MapPut("/{locationId:guid}", async (
            Guid locationId,
            UpdateLocationRequest request,
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new UpdateLocationCommand(
                locationId,
                request.Name,
                request.Type,
                request.City,
                request.Province,
                request.Country,
                request.Address,
                request.Notes,
                currentRequestContext.ModifiedBy), cancellationToken);

            return Results.Ok(response);
        });

        group.MapPut("/{locationId:guid}/activate", async (
            Guid locationId,
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new ActivateLocationCommand(locationId, currentRequestContext.ModifiedBy), cancellationToken);
            return Results.Ok(response);
        });

        group.MapPut("/{locationId:guid}/deactivate", async (
            Guid locationId,
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new DeactivateLocationCommand(locationId, currentRequestContext.ModifiedBy), cancellationToken);
            return Results.Ok(response);
        });

        group.MapDelete("/{locationId:guid}", async (
            Guid locationId,
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new DeleteLocationCommand(locationId, currentRequestContext.ModifiedBy), cancellationToken);
            return Results.Ok(response);
        });

        return app;
    }
}
