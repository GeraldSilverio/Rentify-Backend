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

    public string GetUserId()
    {
        string? userId = _httpContextAccessor.HttpContext?.User
            .FindFirst(ApplicationClaimTypes.UserId)?.Value;

        if (string.IsNullOrWhiteSpace(userId))
            throw new ApiException("User claim is missing or invalid.", StatusCodes.Status401Unauthorized);

        return userId;
    }
}
