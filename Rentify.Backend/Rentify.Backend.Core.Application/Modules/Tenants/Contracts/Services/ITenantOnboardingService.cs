using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Tenants.Commands.RegisterTenant;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Services;

public interface ITenantOnboardingService
{
    Task<ResultReponse<RegisterTenantResponse>> RegisterAsync(
        RegisterTenantCommand request,
        CancellationToken cancellationToken);
}