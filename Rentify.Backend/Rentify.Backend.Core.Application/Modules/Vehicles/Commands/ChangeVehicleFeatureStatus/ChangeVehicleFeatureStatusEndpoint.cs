using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rentify.Backend.Core.Application.Modules.Shared.Constants;
using Rentify.Backend.Core.Application.Modules.Shared.Context;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.ChangeVehicleFeatureStatus;

public static class ChangeVehicleFeatureStatusEndpoint
{
    public static IEndpointRouteBuilder MapChangeVehicleFeatureStatusEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/admin/vehicle-features/{featureId:guid}/activate", async (
            Guid featureId,
            ICurrentUserService currentUserService,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new ChangeVehicleFeatureStatusCommand(featureId, true, currentUserService.ModifiedBy),
                cancellationToken);

            return Results.Ok(response);
        })
        .RequireAuthorization(options => options.RequireRole(ApplicationRoles.SuperAdmin))
        .WithTags("Admin Vehicle Features");

        app.MapPut("/api/v1/admin/vehicle-features/{featureId:guid}/deactivate", async (
            Guid featureId,
            ICurrentUserService currentUserService,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new ChangeVehicleFeatureStatusCommand(featureId, false, currentUserService.ModifiedBy),
                cancellationToken);

            return Results.Ok(response);
        })
        .RequireAuthorization(options => options.RequireRole(ApplicationRoles.SuperAdmin))
        .WithTags("Admin Vehicle Features");

        return app;
    }
}
