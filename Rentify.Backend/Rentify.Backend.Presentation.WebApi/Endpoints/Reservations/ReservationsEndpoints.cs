using MediatR;
using Rentify.Backend.Core.Application.Modules.Reservations.Commands;
using Rentify.Backend.Core.Application.Modules.Reservations.Queries;
using Rentify.Backend.Core.Application.Modules.Shared.Constants;
using Rentify.Backend.Core.Application.Modules.Shared.Context;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Presentation.WebApi.Endpoints.Reservations;

public static class ReservationsEndpoints
{
    public static IEndpointRouteBuilder MapReservationsEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/v1/reservations").WithTags("Reservations").RequireAuthorization(AuthorizationPolicies.RequiredRoles);
        group.MapGet("/availability", async (Guid vehicleId, DateTime deliveryDateTime, DateTime expectedReturnDateTime, Guid? excludeReservationId, ICurrentRequestContext context, ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new CheckVehicleReservationAvailabilityQuery(context.TenantId, vehicleId, deliveryDateTime, expectedReturnDateTime, excludeReservationId), ct)));
        group.MapGet("/", async (string? search, ReservationStatus? status, Guid? customerId, Guid? vehicleId, DateTime? fromDate, DateTime? toDate, ICurrentRequestContext context, ISender sender, CancellationToken ct, int pageNumber = 1, int pageSize = 10) => Results.Ok(await sender.Send(new GetReservationsQuery(context.TenantId, search, status, customerId, vehicleId, fromDate, toDate, pageNumber, pageSize), ct)));
        group.MapGet("/{reservationId:guid}", async (Guid reservationId, ICurrentRequestContext context, ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new GetReservationByIdQuery(context.TenantId, reservationId), ct)));
        group.MapPost("/", async (CreateReservationRequest request, ICurrentRequestContext context, ISender sender, CancellationToken ct) => { var result = await sender.Send(new CreateReservationCommand(context.TenantId, request.CustomerId, request.VehicleId, request.DeliveryDateTime, request.ExpectedReturnDateTime, request.RentalType, request.DeliveryTenantLocationId, request.DeliveryLocationName, request.DeliveryAddressDetails, request.DeliveryFee, request.ReturnTenantLocationId, request.ReturnLocationName, request.ReturnAddressDetails, request.ReturnFee, request.DiscountAmount, request.Channel, request.Notes, context.ModifiedBy), ct); return Results.Created($"/api/v1/reservations/{result.Value!.Id}", result); });
        group.MapPut("/{reservationId:guid}", async (Guid reservationId, UpdateReservationRequest request, ICurrentRequestContext context, ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new UpdateReservationCommand(context.TenantId, reservationId, request.DeliveryDateTime, request.ExpectedReturnDateTime, request.RentalType, request.DeliveryTenantLocationId, request.DeliveryLocationName, request.DeliveryAddressDetails, request.DeliveryFee, request.ReturnTenantLocationId, request.ReturnLocationName, request.ReturnAddressDetails, request.ReturnFee, request.DiscountAmount, request.Notes, context.ModifiedBy), ct)));
        group.MapPut("/{reservationId:guid}/approve", async (Guid reservationId, ICurrentRequestContext context, ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new ApproveReservationCommand(context.TenantId, reservationId, context.ModifiedBy), ct)));
        group.MapPut("/{reservationId:guid}/reject", async (Guid reservationId, ReasonRequest request, ICurrentRequestContext context, ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new RejectReservationCommand(context.TenantId, reservationId, request.Reason, context.ModifiedBy), ct)));
        group.MapPut("/{reservationId:guid}/cancel", async (Guid reservationId, ReasonRequest request, ICurrentRequestContext context, ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new CancelReservationCommand(context.TenantId, reservationId, request.Reason, context.ModifiedBy), ct)));
        group.MapDelete("/{reservationId:guid}", async (Guid reservationId, ICurrentRequestContext context, ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new DeleteReservationCommand(context.TenantId, reservationId, context.ModifiedBy), ct)));
        return app;
    }
}
public sealed record CreateReservationRequest(Guid CustomerId, Guid VehicleId, DateTime DeliveryDateTime, DateTime ExpectedReturnDateTime, RentalType RentalType, Guid? DeliveryTenantLocationId, string? DeliveryLocationName, string? DeliveryAddressDetails, decimal DeliveryFee, Guid? ReturnTenantLocationId, string? ReturnLocationName, string? ReturnAddressDetails, decimal ReturnFee, decimal DiscountAmount, ReservationChannel Channel, string? Notes);
public sealed record UpdateReservationRequest(DateTime DeliveryDateTime, DateTime ExpectedReturnDateTime, RentalType RentalType, Guid? DeliveryTenantLocationId, string? DeliveryLocationName, string? DeliveryAddressDetails, decimal DeliveryFee, Guid? ReturnTenantLocationId, string? ReturnLocationName, string? ReturnAddressDetails, decimal ReturnFee, decimal DiscountAmount, string? Notes);
public sealed record ReasonRequest(string Reason);
