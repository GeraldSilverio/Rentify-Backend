using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Queries.GetReservationById;

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
