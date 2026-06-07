using Rentify.Backend.Core.Application.Modules.Secutiry.Commands.ForgotPassword;
using Rentify.Backend.Core.Application.Modules.Secutiry.Commands.Login;
using Rentify.Backend.Core.Application.Modules.Secutiry.Commands.RefreshToken;
using Rentify.Backend.Core.Application.Modules.Secutiry.Commands.ResetPassword;
using Rentify.Backend.Core.Application.Modules.Secutiry.Commands.RevokeRefreshToken;
using Rentify.Backend.Core.Application.Modules.Secutiry.Dtos.Response;

namespace Rentify.Backend.Core.Application.Modules.Secutiry.Contracts.Services
{
    public interface IAuthenticationService
    {
        Task<LoginResponse> LoginAsync(LoginCommand loginCommand);
        Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordCommand forgotPasswordCommand);
        Task<bool> ResetPasswordAsync(ResetPasswordCommand resetPasswordCommand);
        Task<TokenResponse> RefreshTokenAsync(RefreshTokenCommand refreshTokenCommand);
        Task<bool> RevokeRefreshTokenAsync(RevokeRefreshTokenCommand revokeRefreshTokenCommand);
    }
}
