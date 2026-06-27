using Rentify.Backend.Core.Domain.Entities.Core;

namespace Rentify.Backend.Core.Application.Modules.Core.Contracts.Services
{
    public interface ITenantSettingService
    {
        Task AddAsync(TenantSettings tenantSettings);
    }
}
