using Rentify.Backend.Core.Domain.Entities.Core;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Emails.Contracts.Repositories
{
    public interface ITenantEmailConfigurationRepository
    {
        Task AddAsync(TenantEmailConfiguration tenantEmailConfiguration, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TenantEmailConfiguration>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
        Task<TenantEmailConfiguration?> GetByProviderAsync(Guid tenantId, EmailProviderType provider, CancellationToken cancellationToken = default);
        Task<TenantEmailConfiguration?> GetDefaultAsync(Guid tenantId, CancellationToken cancellationToken = default);
    }
}
