using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Constants;
using Rentify.Backend.Core.Application.Modules.Shared.Context;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.AddVehicleRate;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.DeleteVehicleRate;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UpdateVehicleRate;
using Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicleRateById;
using Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicleRates;

namespace Rentify.Backend.Presentation.WebApi.Endpoints.Vehicles;

public static class VehicleRateEndpoints
{
    public static IEndpointRouteBuilder MapVehicleRateEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app
            .MapGroup("/api/v1/vehicles/{vehicleId:guid}/rates")
            .WithTags("Vehicle Rates")
            .RequireAuthorization(policy => policy.RequireRole(ApplicationRoles.Owner));

        group.MapGet("/", async (
            Guid vehicleId,
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new GetVehicleRatesQuery(currentRequestContext.TenantId, vehicleId),
                cancellationToken);

            return Results.Ok(response);
        })
        .WithName("GetVehicleRates")
        .WithSummary("Gets the active rates for a vehicle.");

        group.MapGet("/{rateId:guid}", async (
            Guid vehicleId,
            Guid rateId,
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new GetVehicleRateByIdQuery(currentRequestContext.TenantId, vehicleId, rateId),
                cancellationToken);

            return Results.Ok(response);
        })
        .WithName("GetVehicleRateById")
        .WithSummary("Gets a vehicle rate for editing.");

        group.MapPost("/", async (
            Guid vehicleId,
            AddVehicleRateRequest request,
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new AddVehicleRateCommand(
                    currentRequestContext.TenantId,
                    vehicleId,
                    request.RentalType,
                    request.Price,
                    currentRequestContext.ModifiedBy),
                cancellationToken);

            return Results.Created($"/api/v1/vehicles/{vehicleId}/rates/{response.Value!.Id}", response);
        })
        .WithName("AddVehicleRate")
        .WithSummary("Adds a rate to a vehicle.");

        group.MapPut("/{rateId:guid}", async (
            Guid vehicleId,
            Guid rateId,
            UpdateVehicleRateRequest request,
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new UpdateVehicleRateCommand(
                    currentRequestContext.TenantId,
                    vehicleId,
                    rateId,
                    request.Price,
                    currentRequestContext.ModifiedBy),
                cancellationToken);

            return Results.Ok(response);
        })
        .WithName("UpdateVehicleRate")
        .WithSummary("Updates the price of a vehicle rate.");

        group.MapDelete("/{rateId:guid}", async (
            Guid vehicleId,
            Guid rateId,
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            await sender.Send(
                new DeleteVehicleRateCommand(
                    currentRequestContext.TenantId,
                    vehicleId,
                    rateId,
                    currentRequestContext.ModifiedBy),
                cancellationToken);

            return Results.NoContent();
        })
        .WithName("DeleteVehicleRate")
        .WithSummary("Soft deletes a vehicle rate.");

        return app;
    }
}
