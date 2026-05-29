using Rentify.Backend.Core.Application.Modules.Secutiry.Dtos.Request;
using Rentify.Backend.Core.Application.Modules.Secutiry.Dtos.Response;
using Rentify.Backend.Core.Application.Modules.Tenants.Commands.RegisterTenant;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Secutiry.Contracts.Services;

public interface IIdentityService
{
    Task<bool> ExistsByEmailAsync(
        string email);

    Task<ResultReponse<Guid>> CreateUserAsync(
        RegisterTenantCommand request,Guid tenantId);

    Task AddToRoleAsync(
        Guid userId,
        string role);

    Task<TokenResponse> GenerateJwtAsync(
        Guid userId);
}