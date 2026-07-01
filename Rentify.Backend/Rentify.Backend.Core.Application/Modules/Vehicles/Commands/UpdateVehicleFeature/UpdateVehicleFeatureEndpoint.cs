using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rentify.Backend.Core.Application.Modules.Shared.Constants;
using Rentify.Backend.Core.Application.Modules.Shared.Context;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UpdateVehicleFeature;

public static class UpdateVehicleFeatureEndpoint
{
    public static IEndpointRouteBuilder MapUpdateVehicleFeatureEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/admin/vehicle-features/{featureId:guid}", async (
            Guid featureId,
            UpdateVehicleFeatureRequest request,
            ICurrentUserService currentUserService,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new UpdateVehicleFeatureCommand(featureId, request.Name, request.Category, currentUserService.ModifiedBy),
                cancellationToken);

            return Results.Ok(response);
        })
        .RequireAuthorization(options => options.RequireRole(ApplicationRoles.SuperAdmin))
        .WithTags("Admin Vehicle Features");

        return app;
    }
}
