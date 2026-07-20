using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Queries.GetReservationById;

public sealed record ReservationCustomerResponse(
    Guid Id,
    string FullName,
    CustomerType CustomerType,
    IdentificationType IdentificationType,
    string IdentificationNumber,
    string PhoneNumber,
    string Email,
    bool IsVerified);
