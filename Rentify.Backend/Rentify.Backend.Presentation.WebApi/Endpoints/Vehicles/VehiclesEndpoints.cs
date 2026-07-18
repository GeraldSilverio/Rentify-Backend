using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Constants;
using Rentify.Backend.Core.Application.Modules.Shared.Context;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.CreateVehicle;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.DeleteVehicle;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.DeleteVehicleImage;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UpdateVehicle;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UploadVehicleImage;
using Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicleDetail;
using Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicleImages;
using Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicles;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Presentation.WebApi.Endpoints.Vehicles
{
    public static class VehiclesEndpoints
    {
        public static IEndpointRouteBuilder MapVehiclesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/v1/vehicles")
                .WithTags("Vehicles")
                .RequireRateLimiting("PublicCatalogPolicy");

            #region POSTS

            group.MapPost("", async (
                  CreateVehicleRequest request,
                  ICurrentRequestContext currentRequestContext,
                  ISender sender,
                  CancellationToken cancellationToken) =>
            {
                var command = new CreateVehicleCommand(
                    currentRequestContext.TenantId,
                    request.VehicleBrandId,
                    request.VehicleModelId,
                    request.VehicleTypeId,
                    request.Year,
                    request.PlateNumber,
                    request.Vin,
                    request.Color,
                    request.CurrentMileage,
                    request.SecurityDepositRequired,
                    request.SecurityDepositAmount,
                    request.Rates,
                    request.FeatureIds ?? Array.Empty<Guid>(),
                    currentRequestContext.ModifiedBy);

                var response = await sender.Send(command, cancellationToken);

                return Results.Created($"/api/v1/vehicles/{response.Value?.Id}", response);
            }).RequireAuthorization(policy => policy.RequireRole(ApplicationRoles.Owner));

            group.MapPost("/{vehicleId:guid}/images", async (
                  Guid vehicleId,
                  HttpRequest request,
                  ICurrentRequestContext currentRequestContext,
                  ISender sender,
                  CancellationToken cancellationToken) =>
            {
                var form = await request.ReadFormAsync(cancellationToken);

                var images = form.Files
                    .Where(file => file.Name.Equals("images", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                var isPrimary = bool.TryParse(form["isPrimary"], out var parsedIsPrimary)
                    && parsedIsPrimary;

                var response = await sender.Send(
                    new UploadVehicleImageCommand(
                        currentRequestContext.TenantId,
                        vehicleId,
                        images,
                        isPrimary,
                        currentRequestContext.ModifiedBy),
                    cancellationToken);

                return Results.Created(
                    $"/api/v1/vehicles/{vehicleId}/images",
                    response);
            }).DisableAntiforgery()
            .RequireAuthorization(policy => policy.RequireRole(ApplicationRoles.Owner));

            #endregion

            #region GETS

            group.MapGet("", async (
                ICurrentRequestContext currentRequestContext,
                ISender sender,
                CancellationToken cancellationToken,
                int pageNumber = 1,
                int pageSize = 10,
                string? search = null,
                Guid? vehicleTypeId = null,
                Guid? vehicleBrandId = null,
                Guid? vehicleModelId = null,
                VehicleStatus? status = null,
                int? year = null,
                decimal? minDailyRate = null,
                decimal? maxDailyRate = null,
                bool? onlyActive = true) =>
            {
                var response = await sender.Send(new GetVehiclesQuery(
                    currentRequestContext.TenantId,
                    pageNumber,
                    pageSize,
                    search,
                    vehicleTypeId,
                    vehicleBrandId,
                    vehicleModelId,
                    status,
                    year,
                    minDailyRate,
                    maxDailyRate,
                    onlyActive), cancellationToken);

                return Results.Ok(response);
            })
            .WithName("GetVehicles")
            .WithSummary("Lists vehicles for a tenant with filters and pagination.");

            group.MapGet("/{vehicleId:guid}", async (
                Guid vehicleId,
                ICurrentRequestContext currentRequestContext,
                ISender sender,
            CancellationToken cancellationToken) =>
            {
                var response = await sender.Send(
                    new GetVehicleDetailQuery(currentRequestContext.TenantId, vehicleId),
                    cancellationToken);

                return Results.Ok(response);
            })
            .WithName("GetVehicleDetail")
            .WithSummary("Gets a vehicle detail for the authenticated tenant.")
            .RequireAuthorization(policy => policy.RequireRole(ApplicationRoles.Owner));

            group.MapGet("/{vehicleId:guid}/images", async (
                Guid vehicleId,
                ICurrentRequestContext currentRequestContext,
                ISender sender,
            CancellationToken cancellationToken) =>
            {
                var response = await sender.Send(
                    new GetVehicleImagesQuery(currentRequestContext.TenantId, vehicleId),
                    cancellationToken);

                return Results.Ok(response);
            }).WithName("GetVehicleImages")
            .WithSummary("Gets a vehicle images for the authenticated tenant.")
            .WithDescription("This endpoint retrieves the images associated with a specific vehicle for the authenticated tenant.");
            #endregion

            #region PUTS

            group.MapPut("/{vehicleId:guid}", async (
                Guid vehicleId,
                UpdateVehicleRequest request,
                ICurrentRequestContext currentRequestContext,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var response = await sender.Send(new UpdateVehicleCommand(
                    currentRequestContext.TenantId,
                    vehicleId,
                    request.VehicleBrandId,
                    request.VehicleModelId,
                    request.VehicleTypeId,
                    request.Year,
                    request.PlateNumber,
                    request.Vin,
                    request.Color,
                    request.CurrentMileage,
                    request.SecurityDepositRequired,
                    request.SecurityDepositAmount,
                    currentRequestContext.ModifiedBy), cancellationToken);

                return Results.Ok(response);
            });

            #endregion

            #region DELETES

            group.MapDelete("/{vehicleId:guid}", async (
            Guid vehicleId,
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
            {
                var response = await sender.Send(
                    new DeleteVehicleCommand(
                        currentRequestContext.TenantId,
                        vehicleId,
                        currentRequestContext.ModifiedBy),
                    cancellationToken);

                return Results.Ok(response);
            }).RequireAuthorization(policy => policy.RequireRole(ApplicationRoles.Owner))
            .WithSummary("Deletes a vehicle for the authenticated tenant.");


            group.MapDelete("/{vehicleId:guid}/images/{imageId:guid}", async (
            Guid vehicleId,
            Guid imageId,
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
            {
                var response = await sender.Send(
                    new DeleteVehicleImageCommand(
                        currentRequestContext.TenantId,
                        vehicleId,
                        imageId,
                        currentRequestContext.ModifiedBy),
                    cancellationToken);

                return Results.Ok(response);
            }).WithSummary("Deletes a vehicle image for the authenticated tenant.")
            .RequireAuthorization(policy => policy.RequireRole(ApplicationRoles.Owner));

            #endregion

            return app;


        }
    }
}
