using Rentify.Backend.Core.Application.Modules.Tenants.Commands.RegisterTenant;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Services;

public interface ITenantOnboardingService
{
    Task<ResultReponse<RegisterTenantResponse>> RegisterAsync(
        RegisterTenantCommand request,
        CancellationToken cancellationToken);
}