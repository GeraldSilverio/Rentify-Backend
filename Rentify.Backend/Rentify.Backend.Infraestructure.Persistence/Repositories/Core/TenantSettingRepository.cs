using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Repositories;
using Rentify.Backend.Core.Domain.Entities.Core;
using Rentify.Backend.Infraestructure.Persistence.Context;

namespace Rentify.Backend.Infraestructure.Persistence.Repositories.Core
{
    public class TenantSettingRepository(RentifyContext context) : ITenantSettingRepository
    {
        public async Task AddAsync(TenantSettings tenantSettings)
        {
            await context.TenantSettings.AddAsync(tenantSettings);
        }

        public async Task<TenantSettings?> GetByTenantIdAsync(
            Guid tenantId,
            CancellationToken cancellationToken = default)
        {
            return await context.TenantSettings
                .FirstOrDefaultAsync(x => x.TenantId == tenantId && !x.IsDeleted, cancellationToken);
        }
    }
}
