using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Queries.GetReservations;

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
