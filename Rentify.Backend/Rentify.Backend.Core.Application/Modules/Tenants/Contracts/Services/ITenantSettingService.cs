using Rentify.Backend.Core.Domain.Entities.Core;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Services
{
    public interface ITenantSettingService
    {
        Task AddAsync(TenantSettings tenantSettings);
    }
}
