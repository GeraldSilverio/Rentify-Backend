using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Constants;
using Rentify.Backend.Core.Application.Modules.Shared.Context;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UpdateVehicleFeatures;
using Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetAssignedVehicleFeatures;

namespace Rentify.Backend.Presentation.WebApi.Endpoints.Vehicles;

public static class VehicleFeatureAssignmentEndpoints
{
    public static IEndpointRouteBuilder MapVehicleFeatureAssignmentEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app
            .MapGroup("/api/v1/vehicles")
            .WithTags("Vehicles Features")
            .RequireAuthorization(policy => policy.RequireRole(ApplicationRoles.Owner));

        group.MapGet("/{vehicleId:guid}/features", async (
            Guid vehicleId,
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new GetAssignedVehicleFeaturesQuery(currentRequestContext.TenantId, vehicleId),
                cancellationToken);

            return Results.Ok(response);
        })
        .WithName("GetAssignedVehicleFeatures")
        .WithSummary("Gets the features assigned to a vehicle.");

        group.MapPut("/{vehicleId:guid}/features", async (
            Guid vehicleId,
            UpdateVehicleFeaturesRequest request,
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new UpdateVehicleFeaturesCommand(
                    currentRequestContext.TenantId,
                    vehicleId,
                    request.FeatureIds,
                    currentRequestContext.ModifiedBy),
                cancellationToken);

            return Results.Ok(response);
        })
        .WithName("UpdateVehicleFeatures")
        .WithSummary("Synchronizes the features assigned to a vehicle.");

        return app;
    }
}

public sealed record UpdateVehicleFeaturesRequest(IReadOnlyCollection<Guid> FeatureIds);
