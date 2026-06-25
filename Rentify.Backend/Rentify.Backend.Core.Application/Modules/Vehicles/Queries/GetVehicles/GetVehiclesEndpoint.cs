using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicles;

public static class GetVehiclesEndpoint
{
    public static IEndpointRouteBuilder MapGetVehiclesEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/tenants/{tenantId:guid}/vehicles", async (
            Guid tenantId,
            ISender sender,
            CancellationToken cancellationToken,
            int pageNumber = 1,
            int pageSize = 10,
            string? search = null,
            Guid? vehicleTypeId = null,
            Guid? vehicleBrandId = null,
            Guid? vehicleModelId = null,
            VehicleStatus? status = null,
            decimal? minDailyRate = null,
            decimal? maxDailyRate = null,
            bool? onlyActive = true) =>
        {
            var response = await sender.Send(new GetVehiclesQuery(
                tenantId,
                pageNumber,
                pageSize,
                search,
                vehicleTypeId,
                vehicleBrandId,
                vehicleModelId,
                status,
                minDailyRate,
                maxDailyRate,
                onlyActive), cancellationToken);

            return Results.Ok(response);
        })
        .WithName("GetVehicles")
        .WithTags("Vehicles")
        .WithSummary("Lists vehicles for a tenant with filters and pagination.");

        return app;
    }
}
