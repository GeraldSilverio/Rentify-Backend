using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Rentify.Backend.Core.Application.Modules.Shared.Constants;
using Rentify.Backend.Core.Application.Modules.Shared.Context;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;

namespace Rentify.Backend.Presentation.WebApi.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            string userId = GetUserId();

            if (!Guid.TryParse(userId, out Guid parsedUserId) || parsedUserId == Guid.Empty)
                throw new ApiException("User claim is missing or invalid.", StatusCodes.Status401Unauthorized);

            return parsedUserId;
        }
    }

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

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true;

    public bool IsSuperAdmin => Roles.Contains(ApplicationRoles.SuperAdmin, StringComparer.OrdinalIgnoreCase);

    public string ModifiedBy
    {
        get
        {
            string? modifiedBy = UserName ?? Email;

            if (!string.IsNullOrWhiteSpace(modifiedBy))
                return modifiedBy;

            return GetUserId();
        }
    }

    public string GetUserId()
    {
        string? userId = GetClaimValue(
            ApplicationClaimTypes.UserId,
            ClaimTypes.NameIdentifier,
            JwtRegisteredClaimNames.Sub,
            "sub");

        if (string.IsNullOrWhiteSpace(userId))
            throw new ApiException("User claim is missing or invalid.", StatusCodes.Status401Unauthorized);

        return userId;
    }

    private string? GetClaimValue(params string[] claimTypes)
    {
        ClaimsPrincipal? user = _httpContextAccessor.HttpContext?.User;

        return claimTypes
            .Select(type => user?.FindFirst(type)?.Value)
            .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));
    }

    private IReadOnlyCollection<string> GetRoleValues()
    {
        ClaimsPrincipal? user = _httpContextAccessor.HttpContext?.User;

        if (user is null)
            return Array.Empty<string>();

        return user.Claims
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
