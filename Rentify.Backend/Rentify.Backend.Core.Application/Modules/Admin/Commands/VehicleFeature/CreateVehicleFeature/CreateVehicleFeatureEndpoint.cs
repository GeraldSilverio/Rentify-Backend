using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rentify.Backend.Core.Application.Modules.Admin.Commands.VehicleFeature.CreateVehicleFeature;
using Rentify.Backend.Core.Application.Modules.Shared.Constants;
using Rentify.Backend.Core.Application.Modules.Shared.Context;

namespace Rentify.Backend.Core.Application.Modules.Admin.Commands.Vehicles.CreateVehicleFeature;

public static class CreateVehicleFeatureEndpoint
{
    public static IEndpointRouteBuilder MapCreateVehicleFeatureEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/admin/vehicle-features", async (
            CreateVehicleFeatureRequest request,
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new CreateVehicleFeatureCommand(request.Name, request.Category, currentRequestContext.ModifiedBy),
                cancellationToken);

            return Results.Created($"/api/v1/admin/vehicle-features/{response.Value?.Id}", response);
        })
        .RequireAuthorization(options => options.RequireRole(ApplicationRoles.SuperAdmin))
        .WithTags("Admin Vehicle Features");

        return app;
    }
}
