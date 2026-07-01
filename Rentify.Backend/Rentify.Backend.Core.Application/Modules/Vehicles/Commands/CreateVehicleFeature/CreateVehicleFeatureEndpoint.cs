using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rentify.Backend.Core.Application.Modules.Shared.Constants;
using Rentify.Backend.Core.Application.Modules.Shared.Context;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.CreateVehicleFeature;

public static class CreateVehicleFeatureEndpoint
{
    public static IEndpointRouteBuilder MapCreateVehicleFeatureEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/admin/vehicle-features", async (
            CreateVehicleFeatureRequest request,
            ICurrentUserService currentUserService,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new CreateVehicleFeatureCommand(request.Name, request.Category, currentUserService.ModifiedBy),
                cancellationToken);

            return Results.Created($"/api/v1/admin/vehicle-features/{response.Value?.Id}", response);
        })
        .RequireAuthorization(options => options.RequireRole(ApplicationRoles.SuperAdmin))
        .WithTags("Admin Vehicle Features");

        return app;
    }
}
