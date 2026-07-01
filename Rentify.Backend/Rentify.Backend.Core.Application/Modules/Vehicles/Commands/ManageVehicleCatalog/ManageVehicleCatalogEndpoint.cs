using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rentify.Backend.Core.Application.Modules.Shared.Constants;
using Rentify.Backend.Core.Application.Modules.Shared.Context;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.ManageVehicleCatalog;

public static class ManageVehicleCatalogEndpoint
{
    public static IEndpointRouteBuilder MapManageVehicleCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        MapBrandEndpoints(app);
        MapModelEndpoints(app);
        MapTypeEndpoints(app);

        return app;
    }

    private static void MapBrandEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/admin/vehicle-brands", (
            ManageVehicleCatalogRequest request,
            ICurrentUserService currentUserService,
            ISender sender,
            CancellationToken cancellationToken) => sender.Send(new ManageVehicleCatalogCommand(
                VehicleCatalogKind.Brand,
                VehicleCatalogAction.Create,
                null,
                request.Name,
                null,
                currentUserService.ModifiedBy), cancellationToken))
            .RequireAuthorization(options => options.RequireRole(ApplicationRoles.SuperAdmin))
            .WithTags("Admin Vehicle Brands");

        app.MapPut("/api/v1/admin/vehicle-brands/{brandId:guid}", (
            Guid brandId,
            ManageVehicleCatalogRequest request,
            ICurrentUserService currentUserService,
            ISender sender,
            CancellationToken cancellationToken) => sender.Send(new ManageVehicleCatalogCommand(
                VehicleCatalogKind.Brand,
                VehicleCatalogAction.Update,
                brandId,
                request.Name,
                null,
                currentUserService.ModifiedBy), cancellationToken))
            .RequireAuthorization(options => options.RequireRole(ApplicationRoles.SuperAdmin))
            .WithTags("Admin Vehicle Brands");

        MapStatusEndpoints(app, "/api/v1/admin/vehicle-brands/{catalogId:guid}", VehicleCatalogKind.Brand, "Admin Vehicle Brands");
    }

    private static void MapModelEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/admin/vehicle-models", (
            ManageVehicleCatalogRequest request,
            ICurrentUserService currentUserService,
            ISender sender,
            CancellationToken cancellationToken) => sender.Send(new ManageVehicleCatalogCommand(
                VehicleCatalogKind.Model,
                VehicleCatalogAction.Create,
                null,
                request.Name,
                request.VehicleBrandId,
                currentUserService.ModifiedBy), cancellationToken))
            .RequireAuthorization(options => options.RequireRole(ApplicationRoles.SuperAdmin))
            .WithTags("Admin Vehicle Models");

        app.MapPut("/api/v1/admin/vehicle-models/{modelId:guid}", (
            Guid modelId,
            ManageVehicleCatalogRequest request,
            ICurrentUserService currentUserService,
            ISender sender,
            CancellationToken cancellationToken) => sender.Send(new ManageVehicleCatalogCommand(
                VehicleCatalogKind.Model,
                VehicleCatalogAction.Update,
                modelId,
                request.Name,
                request.VehicleBrandId,
                currentUserService.ModifiedBy), cancellationToken))
            .RequireAuthorization(options => options.RequireRole(ApplicationRoles.SuperAdmin))
            .WithTags("Admin Vehicle Models");

        MapStatusEndpoints(app, "/api/v1/admin/vehicle-models/{catalogId:guid}", VehicleCatalogKind.Model, "Admin Vehicle Models");
    }

    private static void MapTypeEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/admin/vehicle-types", (
            ManageVehicleCatalogRequest request,
            ICurrentUserService currentUserService,
            ISender sender,
            CancellationToken cancellationToken) => sender.Send(new ManageVehicleCatalogCommand(
                VehicleCatalogKind.Type,
                VehicleCatalogAction.Create,
                null,
                request.Name,
                null,
                currentUserService.ModifiedBy), cancellationToken))
            .RequireAuthorization(options => options.RequireRole(ApplicationRoles.SuperAdmin))
            .WithTags("Admin Vehicle Types");

        app.MapPut("/api/v1/admin/vehicle-types/{typeId:guid}", (
            Guid typeId,
            ManageVehicleCatalogRequest request,
            ICurrentUserService currentUserService,
            ISender sender,
            CancellationToken cancellationToken) => sender.Send(new ManageVehicleCatalogCommand(
                VehicleCatalogKind.Type,
                VehicleCatalogAction.Update,
                typeId,
                request.Name,
                null,
                currentUserService.ModifiedBy), cancellationToken))
            .RequireAuthorization(options => options.RequireRole(ApplicationRoles.SuperAdmin))
            .WithTags("Admin Vehicle Types");

        MapStatusEndpoints(app, "/api/v1/admin/vehicle-types/{catalogId:guid}", VehicleCatalogKind.Type, "Admin Vehicle Types");
    }

    private static void MapStatusEndpoints(
        IEndpointRouteBuilder app,
        string baseRoute,
        VehicleCatalogKind kind,
        string tag)
    {
        app.MapPut($"{baseRoute}/activate", (
            Guid catalogId,
            ICurrentUserService currentUserService,
            ISender sender,
            CancellationToken cancellationToken) => sender.Send(new ManageVehicleCatalogCommand(
                kind,
                VehicleCatalogAction.Activate,
                catalogId,
                null,
                null,
                currentUserService.ModifiedBy), cancellationToken))
            .RequireAuthorization(options => options.RequireRole(ApplicationRoles.SuperAdmin))
            .WithTags(tag);

        app.MapPut($"{baseRoute}/deactivate", (
            Guid catalogId,
            ICurrentUserService currentUserService,
            ISender sender,
            CancellationToken cancellationToken) => sender.Send(new ManageVehicleCatalogCommand(
                kind,
                VehicleCatalogAction.Deactivate,
                catalogId,
                null,
                null,
                currentUserService.ModifiedBy), cancellationToken))
            .RequireAuthorization(options => options.RequireRole(ApplicationRoles.SuperAdmin))
            .WithTags(tag);
    }
}
