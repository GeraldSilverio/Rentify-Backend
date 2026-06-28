using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Services;
using Rentify.Backend.Core.Domain.Entities.Core;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Services
{
    public class TenantSettingService(ITenantSettingRepository tenantSettingRepository) : ITenantSettingService
    {
        public async Task AddAsync(TenantSettings tenantSettings)
        {
            await tenantSettingRepository.AddAsync(tenantSettings);
        }
    }
}
