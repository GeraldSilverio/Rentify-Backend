using Rentify.Backend.Core.Application.Modules.Secutiry.Dtos.Response;

namespace Rentify.Backend.Core.Application.Modules.Secutiry.Commands.Login
{
    public record LoginResponse(
        Guid UserId,
        Guid TenantId,
        string UserName,
        string Email,
        string Fullname,
        List<string> Roles,
        TokenResponse TokenResponse);
}
