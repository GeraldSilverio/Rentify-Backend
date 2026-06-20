using Rentify.Backend.Core.Application.Modules.Secutiry.Dtos.Response;

namespace Rentify.Backend.Core.Application.Modules.Secutiry.Commands.Login
{
    public record LoginResponse(
        UserResponse UserResponse,
        TokenResponse TokenResponse);
}
