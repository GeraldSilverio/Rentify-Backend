using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Commands.CreateReservation;

public static class CreateReservationEndpoint
{
    public static IEndpointRouteBuilder MapCreateReservationEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/tenants/{tenantId:guid}/reservations", async (
            Guid tenantId,
            CreateReservationRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new CreateReservationCommand(
                tenantId,
                request.CustomerId,
                request.VehicleIds,
                request.StartDate,
                request.EndDate,
                request.CreatedBy), cancellationToken);

            return Results.Created($"/api/v1/tenants/{tenantId}/reservations/{response.Value}", response);
        })
        .WithTags("Reservations");

        return app;
    }
}
