using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Presentation.WebApi.Endpoints.Reservations;

public sealed record UpdateReservationRequest(
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
    string? Notes);
