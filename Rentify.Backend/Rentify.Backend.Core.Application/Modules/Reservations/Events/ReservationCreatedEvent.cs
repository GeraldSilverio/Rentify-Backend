using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Events;

public sealed record ReservationCreatedEvent(
    Guid TenantId,
    Guid ReservationId,
    ReservationChannel Channel);
