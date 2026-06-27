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

    public Guid GetTenantId()
    {
        string? tenantId = _httpContextAccessor.HttpContext?.User
            .FindFirst(ApplicationClaimTypes.TenantId)?.Value;

        if (!Guid.TryParse(tenantId, out Guid parsedTenantId) || parsedTenantId == Guid.Empty)
            throw new ApiException("Tenant claim is missing or invalid.", StatusCodes.Status401Unauthorized);

        return parsedTenantId;
    }
}
