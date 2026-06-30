using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Shared.Constants;
using Rentify.Backend.Core.Application.Modules.Shared.Context;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;

namespace Rentify.Backend.Presentation.WebApi.Services;

public sealed class CurrentTenantService : ICurrentTenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentTenantService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid TenantId => GetTenantId();

    public bool HasTenant => TryGetTenantId(out _);

    public Guid GetTenantId()
    {
        if (!TryGetTenantId(out Guid parsedTenantId))
            throw new ApiException("Tenant claim is missing or invalid.", StatusCodes.Status401Unauthorized);

        return parsedTenantId;
    }

    private bool TryGetTenantId(out Guid tenantId)
    {
        string? rawTenantId = _httpContextAccessor.HttpContext?.User
            .FindFirst(ApplicationClaimTypes.TenantId)?.Value
            ?? _httpContextAccessor.HttpContext?.User.FindFirst("TenantId")?.Value
            ?? _httpContextAccessor.HttpContext?.User.FindFirst("tenant_id")?.Value;

        return Guid.TryParse(rawTenantId, out tenantId) && tenantId != Guid.Empty;
    }
}
