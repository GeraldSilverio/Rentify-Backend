namespace Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Services;

public interface ITenantUniquenessService
{
    Task<bool> IsRncInUseAsync(
        string? rnc,
        Guid? excludedTenantId = null,
        CancellationToken cancellationToken = default);

    Task EnsureRncIsUniqueAsync(
        string? rnc,
        Guid? excludedTenantId = null,
        CancellationToken cancellationToken = default);
}
