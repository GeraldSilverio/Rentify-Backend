namespace Rentify.Backend.Core.Application.Modules.Reservations.Contracts.Services;

public sealed record ResolvedReservationLocation(
    Guid? TenantLocationId,
    string Name,
    decimal Fee);

public interface IReservationLocationResolver
{
    Task<ResolvedReservationLocation> ResolveDeliveryAsync(
        Guid tenantId,
        Guid? tenantLocationId,
        string? customName,
        decimal customFee,
        CancellationToken cancellationToken);

    Task<ResolvedReservationLocation> ResolveReturnAsync(
        Guid tenantId,
        Guid? tenantLocationId,
        string? customName,
        decimal customFee,
        CancellationToken cancellationToken);
}
