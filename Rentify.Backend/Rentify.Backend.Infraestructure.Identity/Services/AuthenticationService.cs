using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Rentify.Backend.Core.Application.Modules.Secutiry.Commands.ForgotPassword;
using Rentify.Backend.Core.Application.Modules.Secutiry.Commands.Login;
using Rentify.Backend.Core.Application.Modules.Secutiry.Commands.RefreshToken;
using Rentify.Backend.Core.Application.Modules.Secutiry.Commands.ResetPassword;
using Rentify.Backend.Core.Application.Modules.Secutiry.Commands.RevokeRefreshToken;
using Rentify.Backend.Core.Application.Modules.Secutiry.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Secutiry.Dtos.Response;
using Rentify.Backend.Core.Application.Shared.Exceptions;
using Rentify.Backend.Core.Domain.Settings;
using Rentify.Backend.Infraestructure.Identity.Context;
using Rentify.Backend.Infraestructure.Identity.Contracts.Services;
using Rentify.Backend.Infraestructure.Identity.Entities;

namespace Rentify.Backend.Infraestructure.Identity.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtServices _jwtServices;
        private readonly IdentityContext _identityContext;
        private readonly JwtSettings _jwtSettings;

        public AuthenticationService(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IJwtServices jwtServices,
            IdentityContext identityContext,
            IOptions<JwtSettings> jwtSettings)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtServices = jwtServices;
            _identityContext = identityContext;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<LoginResponse> LoginAsync(LoginCommand loginCommand)
        {
            var user = await FindUserByUserNameOrEmailAsync(loginCommand.UserName);

            if (user == null)
            {
                throw new ApiException("Invalid credentials", StatusCodes.Status401Unauthorized);
            }

            if (!user.IsActive)
            {
                throw new ApiException("User is inactive", StatusCodes.Status403Forbidden);
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, loginCommand.Password, lockoutOnFailure: true);

            if (!signInResult.Succeeded)
            {
                throw new ApiException("Invalid credentials", StatusCodes.Status401Unauthorized);
            }

            var roles = await _userManager.GetRolesAsync(user);
            var tokenResponse = await GenerateTokenResponseAsync(user);

            return new LoginResponse(
                Guid.Parse(user.Id),
                user.TenantId,
                user.UserName!,
                user.Email!,
                user.FullName!,
                roles.ToList(),
                tokenResponse);
        }

        public async Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordCommand forgotPasswordCommand)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordCommand.Email);

            if (user == null) throw new ApiException("User not found", StatusCodes.Status404NotFound);

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            return new ForgotPasswordResponse(user.Email!, resetToken);
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordCommand resetPasswordCommand)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordCommand.Email);

            if (user == null) throw new ApiException("User not found", StatusCodes.Status404NotFound);

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordCommand.Token, resetPasswordCommand.Password);

            if (!result.Succeeded)
            {
                throw new ApiException(string.Join(", ", result.Errors.Select(x => x.Description)), StatusCodes.Status400BadRequest);
            }

            await RevokeAllActiveRefreshTokensAsync(user.Id);

            return true;
        }

        public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenCommand refreshTokenCommand)
        {
            var refreshToken = await _identityContext.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == refreshTokenCommand.RefreshToken);

            if (refreshToken == null || !refreshToken.IsActive)
            {
                throw new ApiException("Invalid refresh token", StatusCodes.Status401Unauthorized);
            }

            var user = await _userManager.FindByIdAsync(refreshToken.UserId);

            if (user == null) throw new ApiException("User not found", StatusCodes.Status404NotFound);

            var newRefreshToken = CreateRefreshToken(user.Id);
            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.ReplacedByToken = newRefreshToken.Token;

            await _identityContext.RefreshTokens.AddAsync(newRefreshToken);
            await _identityContext.SaveChangesAsync();

            return await GenerateTokenResponseAsync(user, newRefreshToken);
        }

        public async Task<bool> RevokeRefreshTokenAsync(RevokeRefreshTokenCommand revokeRefreshTokenCommand)
        {
            var refreshToken = await _identityContext.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == revokeRefreshTokenCommand.RefreshToken);

            if (refreshToken == null) throw new ApiException("Refresh token not found", StatusCodes.Status404NotFound);

            if (!refreshToken.IsRevoked)
            {
                refreshToken.RevokedAt = DateTime.UtcNow;
                await _identityContext.SaveChangesAsync();
            }

            return true;
        }

        private async Task<TokenResponse> GenerateTokenResponseAsync(ApplicationUser user, RefreshToken? refreshToken = null)
        {
            refreshToken ??= CreateRefreshToken(user.Id);

            if (_identityContext.Entry(refreshToken).State == EntityState.Detached)
            {
                await _identityContext.RefreshTokens.AddAsync(refreshToken);
                await _identityContext.SaveChangesAsync();
            }

            var token = await _jwtServices.GenerateSecurityTokenAsync(user);
            var accessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes);

            return new TokenResponse(token, accessTokenExpiresAt, refreshToken.Token, refreshToken.ExpiresAt);
        }

        private RefreshToken CreateRefreshToken(string userId)
        {
            return new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(GetRefreshTokenDurationInDays())
            };
        }

        private async Task RevokeAllActiveRefreshTokensAsync(string userId)
        {
            var activeRefreshTokens = await _identityContext.RefreshTokens
                .Where(x => x.UserId == userId && x.RevokedAt == null && x.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

            foreach (var refreshToken in activeRefreshTokens)
            {
                refreshToken.RevokedAt = DateTime.UtcNow;
            }

            await _identityContext.SaveChangesAsync();
        }

        private int GetRefreshTokenDurationInDays()
        {
            return _jwtSettings.RefreshTokenDurationInDays > 0 ? _jwtSettings.RefreshTokenDurationInDays : 7;
        }

        private async Task<ApplicationUser?> FindUserByUserNameOrEmailAsync(string userNameOrEmail)
        {
            var normalizedValue = userNameOrEmail.Trim();

            var user = await _userManager.FindByNameAsync(normalizedValue);

            if (user != null)
            {
                return user;
            }

            return await _userManager.FindByEmailAsync(normalizedValue);
        }
    }
}
