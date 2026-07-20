using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Dtos;

public sealed record ReservationResponse(
    Guid Id,
    string Code,
    ReservationStatus Status,
    decimal TotalAmount);

public sealed record ReservationListItemResponse(
    Guid Id,
    string Code,
    Guid CustomerId,
    string CustomerName,
    Guid VehicleId,
    string VehicleName,
    string VehiclePlateNumber,
    DateTime DeliveryDateTime,
    DateTime ExpectedReturnDateTime,
    RentalType RentalType,
    int Quantity,
    decimal RentalAmount,
    decimal SecurityDepositAmount,
    decimal DeliveryFee,
    decimal ReturnFee,
    decimal DiscountAmount,
    decimal TotalAmount,
    ReservationStatus Status,
    ReservationChannel Channel,
    DateTime CreatedDate);

public sealed record ReservationCustomerResponse(
    Guid Id,
    string FullName,
    CustomerType CustomerType,
    IdentificationType IdentificationType,
    string IdentificationNumber,
    string PhoneNumber,
    string Email,
    bool IsVerified);

public sealed record ReservationVehicleResponse(
    Guid Id,
    string BrandName,
    string ModelName,
    string TypeName,
    int Year,
    string PlateNumber,
    string Color,
    VehicleStatus Status,
    string? MainImageUrl);

public sealed record ReservationDetailsResponse(
    Guid Id,
    string Code,
    ReservationCustomerResponse Customer,
    ReservationVehicleResponse Vehicle,
    DateTime DeliveryDateTime,
    DateTime ExpectedReturnDateTime,
    RentalType RentalType,
    int Quantity,
    decimal UnitRate,
    decimal RentalAmount,
    bool SecurityDepositRequired,
    decimal SecurityDepositAmount,
    Guid? DeliveryTenantLocationId,
    string DeliveryLocationName,
    string? DeliveryAddressDetails,
    decimal DeliveryFee,
    Guid? ReturnTenantLocationId,
    string ReturnLocationName,
    string? ReturnAddressDetails,
    decimal ReturnFee,
    decimal DiscountAmount,
    decimal TotalAmount,
    ReservationStatus Status,
    ReservationChannel Channel,
    DateTime? ApprovedAt,
    string? ApprovedBy,
    DateTime? RejectedAt,
    string? RejectionReason,
    string? RejectedBy,
    DateTime? CancelledAt,
    string? CancellationReason,
    string? CancelledBy,
    DateTime? ConvertedToRentalAt,
    string? ConvertedToRentalBy,
    string? Notes,
    DateTime CreatedDate,
    DateTime ModifiedDate);

public sealed record ReservationAvailabilityResponse(
    bool Available,
    string? Message);
