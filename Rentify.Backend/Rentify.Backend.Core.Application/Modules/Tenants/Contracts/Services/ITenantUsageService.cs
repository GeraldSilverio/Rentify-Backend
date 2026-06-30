using Rentify.Backend.Core.Application.Modules.Tenants.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Services;

public interface ITenantUsageService
{
    Task<TenantUsageResponse> GetUsageAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task EnsureCanCreateVehicleAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task EnsureCanCreateEmployeeAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task EnsureCanCreateBranchAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task EnsureCanCreateReservationAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task EnsureFeatureEnabledAsync(
        Guid tenantId,
        string featureCode,
        CancellationToken cancellationToken = default);
}
