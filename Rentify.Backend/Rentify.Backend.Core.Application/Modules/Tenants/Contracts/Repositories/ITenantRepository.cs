using Rentify.Backend.Core.Domain.Entities.Core;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Repositories;

public interface ITenantRepository
{
    Task<bool> EmailExistAsync(string email, Guid? excludedTenantId = null, CancellationToken cancellationToken = default);
    Task<bool> PhoneNumberExistAsync(string phoneNumber, Guid? excludedTenantId = null, CancellationToken cancellationToken = default);
    Task AddAsync(
        Tenant tenant,
        CancellationToken cancellationToken = default);

    Task<Tenant?> GetByIdAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<bool> RncExistsAsync(
        string normalizedRnc,
        Guid? excludedTenantId = null,
        CancellationToken cancellationToken = default);

    Task<bool> RncExistsForAnotherTenantAsync(
        Guid tenantId,
        string normalizedRnc,
        CancellationToken cancellationToken = default);

    Task<bool> IsTenantActiveAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);
}
