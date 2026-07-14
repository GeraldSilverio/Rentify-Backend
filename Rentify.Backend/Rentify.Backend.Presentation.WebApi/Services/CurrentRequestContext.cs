using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Rentify.Backend.Core.Application.Modules.Shared.Constants;
using Rentify.Backend.Core.Application.Modules.Shared.Context;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;

namespace Rentify.Backend.Presentation.WebApi.Services;

public sealed class CurrentRequestContext : ICurrentRequestContext
{
    private readonly ClaimsPrincipal? _user;

    public CurrentRequestContext(IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor);

        _user = httpContextAccessor.HttpContext?.User;
    }

    public bool IsAuthenticated => _user?.Identity?.IsAuthenticated == true;

    public Guid UserId => GetRequiredGuid(
        "User claim is missing or invalid.",
        ApplicationClaimTypes.UserId,
        ClaimTypes.NameIdentifier,
        JwtRegisteredClaimNames.Sub,
        "sub");

    public Guid TenantId => GetRequiredGuid(
        "Tenant claim is missing or invalid.",
        ApplicationClaimTypes.TenantId,
        "TenantId",
        "tenant_id");

    public bool HasTenant => TryGetTenantId(out _);

    public string? UserName => GetClaimValue(
        ClaimTypes.Name,
        JwtRegisteredClaimNames.UniqueName,
        "preferred_username",
        "name");

    public string? Email => GetClaimValue(
        ClaimTypes.Email,
        JwtRegisteredClaimNames.Email,
        "email");

    public IReadOnlyCollection<string> Roles => GetRoleValues();

    public bool IsSuperAdmin => Roles.Contains(ApplicationRoles.SuperAdmin, StringComparer.OrdinalIgnoreCase);

    public string ModifiedBy
    {
        get
        {
            string? modifiedBy = UserName ?? Email;

            if (!string.IsNullOrWhiteSpace(modifiedBy))
                return modifiedBy;

            return UserId.ToString();
        }
    }

    public bool TryGetTenantId(out Guid tenantId)
    {
        string? rawTenantId = GetClaimValue(
            ApplicationClaimTypes.TenantId,
            "TenantId",
            "tenant_id");

        return Guid.TryParse(rawTenantId, out tenantId) && tenantId != Guid.Empty;
    }

    private Guid GetRequiredGuid(string message, params string[] claimTypes)
    {
        string? value = GetClaimValue(claimTypes);

        if (!Guid.TryParse(value, out Guid parsedValue) || parsedValue == Guid.Empty)
            throw new ApiException(message, StatusCodes.Status401Unauthorized);

        return parsedValue;
    }

    private string? GetClaimValue(params string[] claimTypes)
    {
        return claimTypes
            .Select(type => _user?.FindFirst(type)?.Value)
            .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));
    }

    private IReadOnlyCollection<string> GetRoleValues()
    {
        if (_user is null)
            return Array.Empty<string>();

        return _user.Claims
            .Where(claim =>
                claim.Type == ApplicationClaimTypes.Roles ||
                claim.Type == ClaimTypes.Role ||
                claim.Type.Equals("role", StringComparison.OrdinalIgnoreCase) ||
                claim.Type.Equals("roles", StringComparison.OrdinalIgnoreCase))
            .SelectMany(claim => claim.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}
