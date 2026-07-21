using System.Security.Cryptography;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Rentify.Backend.Core.Application.Modules.Emails.Commands.SendTemplateEmail;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Secutiry;
using Rentify.Backend.Core.Application.Modules.Secutiry.Commands.ForgotPassword;
using Rentify.Backend.Core.Application.Modules.Secutiry.Commands.Login;
using Rentify.Backend.Core.Application.Modules.Secutiry.Commands.RefreshToken;
using Rentify.Backend.Core.Application.Modules.Secutiry.Commands.ResetPassword;
using Rentify.Backend.Core.Application.Modules.Secutiry.Commands.RevokeRefreshToken;
using Rentify.Backend.Core.Application.Modules.Secutiry.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Secutiry.Dtos.Response;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Domain.Settings;
using Rentify.Backend.Infraestructure.Identity.Context;
using Rentify.Backend.Infraestructure.Identity.Contracts.Services;
using Rentify.Backend.Infraestructure.Identity.Entities;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Repositories;

namespace Rentify.Backend.Infraestructure.Identity.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtServices _jwtServices;
        private readonly IdentityContext _identityContext;
        private readonly JwtSettings _jwtSettings;
        private readonly IEmailService _emailService;
        private readonly ITenantRepository _tenantRepository;

        public AuthenticationService(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IJwtServices jwtServices,
            IdentityContext identityContext,
            IOptions<JwtSettings> jwtSettings,
            IEmailService emailService,
            ITenantRepository tenantRepository)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtServices = jwtServices;
            _identityContext = identityContext;
            _jwtSettings = jwtSettings.Value;
            _emailService = emailService;
            _tenantRepository = tenantRepository;
        }

        public async Task<LoginResponse> LoginAsync(LoginCommand loginCommand)
        {
            var user = await FindUserByUserNameOrEmailAsync(loginCommand.UserName);

            if (user == null)
            {
                throw new ApiException("Crendenciales inválidas", StatusCodes.Status401Unauthorized);
            }

            if (!user.IsActive)
            {
                throw new ApiException("Usuario desactivado, favor contactar al administrador.", StatusCodes.Status403Forbidden);
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, loginCommand.Password, lockoutOnFailure: true);

            if (!signInResult.Succeeded)
            {
                throw new ApiException("Crendenciales inválidas", StatusCodes.Status401Unauthorized);
            }

            if (!await _tenantRepository.IsTenantActiveAsync(user.TenantId))
            {
                throw new ApiException("La empresa no esta activa, favor contactar al administrador.", StatusCodes.Status403Forbidden);
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

            const string responseMessage = "If the email exists, a password reset link has been sent.";

            if (user == null || !user.IsActive)
            {
                return new ForgotPasswordResponse(responseMessage);
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetUrl = BuildResetPasswordUrl(forgotPasswordCommand.ResetPasswordUrl, user.Email!, resetToken);

            await _emailService.SendEmailAsync(new SendTemplateEmailCommand(
                user.TenantId,
                EmailTemplateCodes.PasswordReset,
                user.Email!,
                new Dictionary<string, string>
                {
                    ["FullName"] = user.FullName ?? user.UserName ?? user.Email!,
                    ["Email"] = user.Email!,
                    ["ResetToken"] = resetToken,
                    ["ResetUrl"] = resetUrl
                }));

            return new ForgotPasswordResponse(responseMessage);
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordCommand resetPasswordCommand)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordCommand.Email);

            if (user == null)
            {
                throw new ApiException("Invalid reset password request", StatusCodes.Status400BadRequest);
            }

            var isValidToken = await _userManager.VerifyUserTokenAsync(
                user,
                _userManager.Options.Tokens.PasswordResetTokenProvider,
                UserManager<ApplicationUser>.ResetPasswordTokenPurpose,
                resetPasswordCommand.Token);

            if (!isValidToken)
            {
                throw new ApiException("Invalid reset password token", StatusCodes.Status400BadRequest);
            }

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
            string refreshTokenHash = HashRefreshToken(refreshTokenCommand.RefreshToken);

            var refreshToken = await _identityContext.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == refreshTokenHash);

            if (refreshToken == null)
            {
                throw new ApiException(
                    "El refresh token no es válido.",
                    StatusCodes.Status401Unauthorized,
                    "INVALID_REFRESH_TOKEN");
            }

            if (refreshToken.IsExpired)
            {
                throw new ApiException(
                    "Tu refresh token ha vencido.",
                    StatusCodes.Status401Unauthorized,
                    "REFRESH_TOKEN_EXPIRED");
            }

            if (refreshToken.IsRevoked)
            {
                if (!string.IsNullOrWhiteSpace(refreshToken.ReplacedByToken))
                {
                    await RevokeAllActiveRefreshTokensAsync(refreshToken.UserId);
                }

                throw new ApiException(
                    "El refresh token no es válido.",
                    StatusCodes.Status401Unauthorized,
                    "INVALID_REFRESH_TOKEN");
            }

            var user = await _userManager.FindByIdAsync(refreshToken.UserId);

            if (user == null || !user.IsActive)
            {
                throw new ApiException(
                    "El refresh token no es válido.",
                    StatusCodes.Status401Unauthorized,
                    "INVALID_REFRESH_TOKEN");
            }

            if (!await _tenantRepository.IsTenantActiveAsync(user.TenantId))
            {
                throw new ApiException(
                    "La empresa no está activa, favor contactar al administrador.",
                    StatusCodes.Status403Forbidden);
            }

            GeneratedRefreshToken newRefreshToken = CreateRefreshToken(user.Id);
            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.ReplacedByToken = newRefreshToken.Entity.Token;

            await _identityContext.RefreshTokens.AddAsync(newRefreshToken.Entity);
            await _identityContext.SaveChangesAsync();

            return await GenerateTokenResponseAsync(user, newRefreshToken);
        }

        public async Task<bool> RevokeRefreshTokenAsync(RevokeRefreshTokenCommand revokeRefreshTokenCommand)
        {
            string refreshTokenHash = HashRefreshToken(revokeRefreshTokenCommand.RefreshToken);

            var refreshToken = await _identityContext.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == refreshTokenHash);

            if (refreshToken == null)
            {
                throw new ApiException(
                    "El refresh token no es válido.",
                    StatusCodes.Status401Unauthorized,
                    "INVALID_REFRESH_TOKEN");
            }

            if (!refreshToken.IsRevoked)
            {
                refreshToken.RevokedAt = DateTime.UtcNow;
                await _identityContext.SaveChangesAsync();
            }

            return true;
        }

        private async Task<TokenResponse> GenerateTokenResponseAsync(
            ApplicationUser user,
            GeneratedRefreshToken? refreshToken = null)
        {
            refreshToken ??= CreateRefreshToken(user.Id);

            if (_identityContext.Entry(refreshToken.Entity).State == EntityState.Detached)
            {
                await _identityContext.RefreshTokens.AddAsync(refreshToken.Entity);
                await _identityContext.SaveChangesAsync();
            }

            var token = await _jwtServices.GenerateSecurityTokenAsync(user);
            var accessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes);

            return new TokenResponse(
                token,
                accessTokenExpiresAt,
                refreshToken.RawToken,
                refreshToken.Entity.ExpiresAt);
        }

        private GeneratedRefreshToken CreateRefreshToken(string userId)
        {
            string rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            RefreshToken refreshToken = new()
            {
                Id = Guid.NewGuid(),
                Token = HashRefreshToken(rawToken),
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(GetRefreshTokenDurationInDays())
            };

            return new GeneratedRefreshToken(refreshToken, rawToken);
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

        private static string BuildResetPasswordUrl(string baseResetPasswordUrl, string email, string token)
        {
            var separator = baseResetPasswordUrl.Contains('?') ? "&" : "?";

            return $"{baseResetPasswordUrl}{separator}email={WebUtility.UrlEncode(email)}&token={WebUtility.UrlEncode(token)}";
        }

        private static string HashRefreshToken(string refreshToken)
        {
            byte[] tokenBytes = System.Text.Encoding.UTF8.GetBytes(refreshToken);
            byte[] hashBytes = SHA256.HashData(tokenBytes);

            return Convert.ToHexString(hashBytes);
        }

        private sealed record GeneratedRefreshToken(
            RefreshToken Entity,
            string RawToken);
    }
}
