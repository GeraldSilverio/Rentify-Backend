using MediatR;
using Rentify.Backend.Core.Application.Modules.Reservations.Commands.ApproveReservation;
using Rentify.Backend.Core.Application.Modules.Reservations.Commands.CancelReservation;
using Rentify.Backend.Core.Application.Modules.Reservations.Commands.CreateReservation;
using Rentify.Backend.Core.Application.Modules.Reservations.Commands.DeleteReservation;
using Rentify.Backend.Core.Application.Modules.Reservations.Commands.RejectReservation;
using Rentify.Backend.Core.Application.Modules.Reservations.Commands.UpdateReservation;
using Rentify.Backend.Core.Application.Modules.Reservations.Dtos;
using Rentify.Backend.Core.Application.Modules.Reservations.Queries.CheckVehicleReservationAvailability;
using Rentify.Backend.Core.Application.Modules.Reservations.Queries.GetReservationById;
using Rentify.Backend.Core.Application.Modules.Reservations.Queries.GetReservations;
using Rentify.Backend.Core.Application.Modules.Shared.Constants;
using Rentify.Backend.Core.Application.Modules.Shared.Context;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Presentation.WebApi.Endpoints.Reservations;

public static class ReservationsEndpoints
{
    public static IEndpointRouteBuilder MapReservationsEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app
            .MapGroup("/api/v1/reservations")
            .WithTags("Reservations")
            .RequireAuthorization(AuthorizationPolicies.RequiredRoles);

        group.MapGet(
            "/availability",
            async (
                Guid vehicleId,
                DateTime deliveryDateTime,
                DateTime expectedReturnDateTime,
                Guid? excludeReservationId,
                ICurrentRequestContext context,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                CheckVehicleReservationAvailabilityQuery query = new(
                    context.TenantId,
                    vehicleId,
                    deliveryDateTime,
                    expectedReturnDateTime,
                    excludeReservationId);

                return Results.Ok(await sender.Send(query, cancellationToken));
            });

        group.MapGet(
            "/",
            async (
                string? search,
                ReservationStatus? status,
                Guid? customerId,
                Guid? vehicleId,
                DateTime? fromDate,
                DateTime? toDate,
                ICurrentRequestContext context,
                ISender sender,
                CancellationToken cancellationToken,
                int pageNumber = 1,
                int pageSize = 10) =>
            {
                GetReservationsQuery query = new(
                    context.TenantId,
                    search,
                    status,
                    customerId,
                    vehicleId,
                    fromDate,
                    toDate,
                    pageNumber,
                    pageSize);

                return Results.Ok(await sender.Send(query, cancellationToken));
            });

        group.MapGet(
            "/{reservationId:guid}",
            async (
                Guid reservationId,
                ICurrentRequestContext context,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                GetReservationByIdQuery query = new(
                    context.TenantId,
                    reservationId);

                return Results.Ok(await sender.Send(query, cancellationToken));
            });

        group.MapPost(
            "/",
            async (
                CreateReservationRequest request,
                ICurrentRequestContext context,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                CreateReservationCommand command = new(
                    context.TenantId,
                    request.CustomerId,
                    request.VehicleId,
                    request.DeliveryDateTime,
                    request.ExpectedReturnDateTime,
                    request.RentalType,
                    request.DeliveryTenantLocationId,
                    request.DeliveryLocationName,
                    request.DeliveryAddressDetails,
                    request.DeliveryFee,
                    request.ReturnTenantLocationId,
                    request.ReturnLocationName,
                    request.ReturnAddressDetails,
                    request.ReturnFee,
                    request.DiscountAmount,
                    request.Channel,
                    request.Notes,
                    context.ModifiedBy);

                ResultReponse<ReservationResponse> result =
                    await sender.Send(command, cancellationToken);

                return Results.Created(
                    $"/api/v1/reservations/{result.Value!.Id}",
                    result);
            });

        group.MapPut(
            "/{reservationId:guid}",
            async (
                Guid reservationId,
                UpdateReservationRequest request,
                ICurrentRequestContext context,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                UpdateReservationCommand command = new(
                    context.TenantId,
                    reservationId,
                    request.CustomerId,
                    request.VehicleId,
                    request.DeliveryDateTime,
                    request.ExpectedReturnDateTime,
                    request.RentalType,
                    request.DeliveryTenantLocationId,
                    request.DeliveryLocationName,
                    request.DeliveryAddressDetails,
                    request.DeliveryFee,
                    request.ReturnTenantLocationId,
                    request.ReturnLocationName,
                    request.ReturnAddressDetails,
                    request.ReturnFee,
                    request.DiscountAmount,
                    request.Notes,
                    context.ModifiedBy);

                return Results.Ok(await sender.Send(command, cancellationToken));
            });

        group.MapPut(
            "/{reservationId:guid}/approve",
            async (
                Guid reservationId,
                ICurrentRequestContext context,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                ApproveReservationCommand command = new(
                    context.TenantId,
                    reservationId,
                    context.UserName);

                return Results.Ok(await sender.Send(command, cancellationToken));
            });

        group.MapPut(
            "/{reservationId:guid}/reject",
            async (
                Guid reservationId,
                ReasonRequest request,
                ICurrentRequestContext context,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                RejectReservationCommand command = new(
                    context.TenantId,
                    reservationId,
                    request.Reason,
                    context.ModifiedBy);

                return Results.Ok(await sender.Send(command, cancellationToken));
            });

        group.MapPut(
            "/{reservationId:guid}/cancel",
            async (
                Guid reservationId,
                ReasonRequest request,
                ICurrentRequestContext context,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                CancelReservationCommand command = new(
                    context.TenantId,
                    reservationId,
                    request.Reason,
                    context.ModifiedBy);

                return Results.Ok(await sender.Send(command, cancellationToken));
            });

        group.MapDelete(
            "/{reservationId:guid}",
            async (
                Guid reservationId,
                ICurrentRequestContext context,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                DeleteReservationCommand command = new(
                    context.TenantId,
                    reservationId,
                    context.ModifiedBy);

                return Results.Ok(await sender.Send(command, cancellationToken));
            });

        return app;
    }
}
