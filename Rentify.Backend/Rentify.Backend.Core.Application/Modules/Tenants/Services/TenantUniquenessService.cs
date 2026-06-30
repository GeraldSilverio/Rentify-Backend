using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Tenants.Validation;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Services;

public sealed class TenantUniquenessService : ITenantUniquenessService
{
    private const string RncAlreadyInUseMessage = "Este RNC ya está en uso por otra empresa.";

    private readonly ITenantRepository _tenantRepository;

    public TenantUniquenessService(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<bool> IsRncInUseAsync(
        string? rnc,
        Guid? excludedTenantId = null,
        CancellationToken cancellationToken = default)
    {
        string? normalizedRnc = TenantRncRules.NormalizeOrNull(rnc);

        return normalizedRnc is not null &&
               await _tenantRepository.RncExistsAsync(normalizedRnc, excludedTenantId, cancellationToken);
    }

    public async Task EnsureRncIsUniqueAsync(
        string? rnc,
        Guid? excludedTenantId = null,
        CancellationToken cancellationToken = default)
    {
        if (await IsRncInUseAsync(rnc, excludedTenantId, cancellationToken))
            throw new ApiException(RncAlreadyInUseMessage, StatusCodes.Status400BadRequest);
    }
}
