using Rentify.Backend.Core.Domain.Entities.Core;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Repositories
{
    public interface ITenantSettingRepository
    {
        Task AddAsync(TenantSettings tenantSettings);

        Task<TenantSettings?> GetByTenantIdAsync(
            Guid tenantId,
            CancellationToken cancellationToken = default);
    }
}
