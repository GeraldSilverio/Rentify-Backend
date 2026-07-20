using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Presentation.WebApi.Endpoints.Reservations;

public sealed record CreateReservationRequest(
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
    string? Notes);
