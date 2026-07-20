using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Dtos;

public sealed record ReservationResponse(
    Guid Id,
    string Code,
    ReservationStatus Status,
    decimal TotalAmount);
