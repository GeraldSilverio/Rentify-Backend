using Rentify.Backend.Core.Application.Modules.Tenants.Commands.RegisterTenant;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Services
{
    public interface ITenantService
    {
        Task<RegisterTenantResponse> CreateTenantAsync(RegisterTenantCommand registerTenantCommand,CancellationToken cancellationToken);
    }
}
