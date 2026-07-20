using MediatR;
using Rentify.Backend.Core.Application.Modules.Reservations.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Commands;

public sealed record CreateReservationCommand(
    Guid TenantId,
    Guid CustomerId,
    Guid VehicleId,
    DateTime DeliveryDateTime,
    DateTime ExpectedReturnDateTime,
    RentalType RentalType,
    Guid? DeliveryTenantLocationId,
    string? DeliveryLocationName,
    string? DeliveryAddressDetails,
    decimal DeliveryFee,
    Guid? ReturnTenantLocationId,
    string? ReturnLocationName,
    string? ReturnAddressDetails,
    decimal ReturnFee,
    decimal DiscountAmount,
    ReservationChannel Channel,
    string? Notes,
    string CreatedBy) : IRequest<ResultReponse<ReservationResponse>>;
