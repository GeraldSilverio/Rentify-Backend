namespace Rentify.Backend.Core.Application.Modules.Reservations.Events;

public sealed record ReservationApprovedOutboxPayload(
    Guid TenantId,
    Guid ReservationId,
    DateTime ApprovedAtUtc);
