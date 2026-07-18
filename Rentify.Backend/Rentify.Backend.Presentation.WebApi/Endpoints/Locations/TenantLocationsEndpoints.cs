using MediatR;
using Rentify.Backend.Core.Application.Modules.Locations.Commands.ActivateTenantLocation;
using Rentify.Backend.Core.Application.Modules.Locations.Commands.CreateTenantLocation;
using Rentify.Backend.Core.Application.Modules.Locations.Commands.DeactivateTenantLocation;
using Rentify.Backend.Core.Application.Modules.Locations.Commands.DeleteTenantLocation;
using Rentify.Backend.Core.Application.Modules.Locations.Commands.UpdateTenantLocation;
using Rentify.Backend.Core.Application.Modules.Locations.Queries.GetTenantLocationById;
using Rentify.Backend.Core.Application.Modules.Locations.Queries.GetTenantLocations;
using Rentify.Backend.Core.Application.Modules.Shared.Constants;
using Rentify.Backend.Core.Application.Modules.Shared.Context;

namespace Rentify.Backend.Presentation.WebApi.Endpoints.Locations;

public static class TenantLocationsEndpoints
{
    public static IEndpointRouteBuilder MapTenantLocationsEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app
            .MapGroup("/api/v1/tenant-locations")
            .WithTags("Tenant Locations")
            .RequireAuthorization(AuthorizationPolicies.RequiredRoles);

        group.MapGet("/", async (
            string? search,
            bool? isActive,
            bool? supportsDelivery,
            bool? supportsPickup,
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken,
            int pageNumber = 1,
            int pageSize = 10) =>
        {
            var response = await sender.Send(new GetTenantLocationsQuery(
                currentRequestContext.TenantId,
                search,
                isActive,
                supportsDelivery,
                supportsPickup,
                pageNumber,
                pageSize), cancellationToken);

            return Results.Ok(response);
        });

        group.MapGet("/{tenantLocationId:guid}", async (
            Guid tenantLocationId,
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new GetTenantLocationByIdQuery(
                currentRequestContext.TenantId,
                tenantLocationId), cancellationToken);

            return Results.Ok(response);
        });

        group.MapPost("/", async (
            CreateTenantLocationRequest request,
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new CreateTenantLocationCommand(
                currentRequestContext.TenantId,
                request.LocationId,
                request.DisplayName,
                request.AllowsDelivery,
                request.AllowsPickup,
                request.DeliveryFee,
                request.PickupFee,
                request.IsCustom,
                currentRequestContext.ModifiedBy), cancellationToken);

            return Results.Created($"/api/v1/tenant-locations/{response.Value?.Id}", response);
        });

        group.MapPut("/{tenantLocationId:guid}", async (
            Guid tenantLocationId,
            UpdateTenantLocationRequest request,
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new UpdateTenantLocationCommand(
                currentRequestContext.TenantId,
                tenantLocationId,
                request.DisplayName,
                request.AllowsDelivery,
                request.AllowsPickup,
                request.DeliveryFee,
                request.PickupFee,
                currentRequestContext.ModifiedBy), cancellationToken);

            return Results.Ok(response);
        });

        group.MapPut("/{tenantLocationId:guid}/activate", async (
            Guid tenantLocationId,
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new ActivateTenantLocationCommand(
                currentRequestContext.TenantId,
                tenantLocationId,
                currentRequestContext.ModifiedBy), cancellationToken);

            return Results.Ok(response);
        });

        group.MapPut("/{tenantLocationId:guid}/deactivate", async (
            Guid tenantLocationId,
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new DeactivateTenantLocationCommand(
                currentRequestContext.TenantId,
                tenantLocationId,
                currentRequestContext.ModifiedBy), cancellationToken);

            return Results.Ok(response);
        });

        group.MapDelete("/{tenantLocationId:guid}", async (
            Guid tenantLocationId,
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new DeleteTenantLocationCommand(
                currentRequestContext.TenantId,
                tenantLocationId,
                currentRequestContext.ModifiedBy), cancellationToken);

            return Results.Ok(response);
        });

        return app;
    }
}
