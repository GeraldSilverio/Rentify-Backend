using Rentify.Backend.Core.Domain.Enums;
using Rentify.Backend.Core.Domain.ValueObjects;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Dtos;

public sealed record ReservationApprovedEmailData(
    string CustomerFirstName,
    string CustomerEmail,
    string TenantName,
    PhoneNumber TenantPhone,
    PhoneNumber TenantWhatsApp,
    Email TenantEmail,
    string ReservationCode,
    DateTime ApprovedAtUtc,
    string VehicleBrandName,
    string VehicleModelName,
    int VehicleYear,
    RentalType RentalType,
    int Quantity,
    DateTime DeliveryDateTime,
    DateTime ExpectedReturnDateTime,
    string DeliveryLocation,
    string ReturnLocation,
    decimal UnitRate,
    decimal RentalAmount,
    decimal DeliveryFee,
    decimal ReturnFee,
    bool SecurityDepositRequired,
    decimal SecurityDepositAmount,
    decimal DiscountAmount,
    decimal TotalAmount);
