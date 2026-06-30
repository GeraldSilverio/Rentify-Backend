using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Services;
using Rentify.Backend.Core.Domain.Entities.Core;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Services;

public sealed class TenantAccessService : ITenantAccessService
{
    private const string InactiveTenantMessage = "La empresa está inactiva. Contacte al administrador de Rentify.";

    private readonly ITenantRepository _tenantRepository;

    public TenantAccessService(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<Tenant> GetTenantAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _tenantRepository.GetByIdAsync(tenantId, cancellationToken)
            ?? throw new ApiException("Tenant not found.", StatusCodes.Status404NotFound);
    }

    public async Task<Tenant> GetActiveTenantAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        Tenant tenant = await GetTenantAsync(tenantId, cancellationToken);

        if (!tenant.IsActive)
            throw new ApiException(InactiveTenantMessage, StatusCodes.Status403Forbidden);

        return tenant;
    }

    public async Task EnsureTenantIsActiveAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        _ = await GetActiveTenantAsync(tenantId, cancellationToken);
    }

    public async Task<bool> IsTenantActiveAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        Tenant? tenant = await _tenantRepository.GetByIdAsync(tenantId, cancellationToken);

        return tenant?.IsActive == true;
    }
}
