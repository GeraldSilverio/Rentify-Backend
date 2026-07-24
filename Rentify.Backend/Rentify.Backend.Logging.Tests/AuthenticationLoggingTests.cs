using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rentify.Backend.Core.Application.Modules.Emails.Commands.SendTemplateEmail;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Secutiry.Commands.ForgotPassword;
using Rentify.Backend.Core.Application.Modules.Secutiry.Commands.ResetPassword;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Domain.Settings;
using Rentify.Backend.Infraestructure.Identity.Entities;
using Rentify.Backend.Infraestructure.Identity.Services;
using Serilog;
using Serilog.Core;

namespace Rentify.Backend.Logging.Tests;

public sealed class AuthenticationLoggingTests
{
    private const string PrivateEmail = "private.user@example.test";
    private const string ResetToken = "reset-token-do-not-log";
    private const string Password = "password-do-not-log";

    [Fact]
    public async Task ForgotPasswordDoesNotLogEmailTokenOrResetUrl()
    {
        TestLogSink sink = new();
        using Logger serilogLogger = CreateLogger(sink);
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            builder.AddSerilog(serilogLogger, dispose: false));
        ApplicationUser user = CreateUser();
        FakeEmailService emailService = new();
        using FakeUserManager userManager = new(user, validResetToken: false);
        AuthenticationService service = CreateService(
            userManager,
            emailService,
            loggerFactory.CreateLogger<AuthenticationService>());

        await service.ForgotPasswordAsync(new ForgotPasswordCommand(
            PrivateEmail,
            "https://frontend.example.test/reset-password"));

        string logs = sink.RenderAll();
        Assert.NotNull(emailService.LastCommand);
        Assert.DoesNotContain(PrivateEmail, logs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(ResetToken, logs, StringComparison.Ordinal);
        Assert.DoesNotContain("reset-password", logs, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task InvalidResetPasswordDoesNotLogEmailTokenOrPasswords()
    {
        TestLogSink sink = new();
        using Logger serilogLogger = CreateLogger(sink);
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            builder.AddSerilog(serilogLogger, dispose: false));
        using FakeUserManager userManager = new(CreateUser(), validResetToken: false);
        AuthenticationService service = CreateService(
            userManager,
            new FakeEmailService(),
            loggerFactory.CreateLogger<AuthenticationService>());

        await Assert.ThrowsAsync<ApiException>(() => service.ResetPasswordAsync(
            new ResetPasswordCommand(
                PrivateEmail,
                ResetToken,
                Password,
                Password)));

        string logs = sink.RenderAll();
        Assert.DoesNotContain(PrivateEmail, logs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(ResetToken, logs, StringComparison.Ordinal);
        Assert.DoesNotContain(Password, logs, StringComparison.Ordinal);
    }

    private static Logger CreateLogger(TestLogSink sink)
    {
        return new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Sink(sink)
            .CreateLogger();
    }

    private static ApplicationUser CreateUser()
    {
        return new ApplicationUser
        {
            Id = TestIdentifiers.UserId.ToString(),
            TenantId = TestIdentifiers.TenantId,
            Email = PrivateEmail,
            UserName = "test-user",
            FullName = "Test User",
            IsActive = true
        };
    }

    private static AuthenticationService CreateService(
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        ILogger<AuthenticationService> logger)
    {
        return new AuthenticationService(
            null!,
            userManager,
            null!,
            null!,
            Options.Create(new JwtSettings()),
            emailService,
            null!,
            logger);
    }

    private sealed class FakeEmailService : IEmailService
    {
        public SendTemplateEmailCommand? LastCommand { get; private set; }

        public Task<SendTemplateEmailResponse> SendEmailAsync(
            SendTemplateEmailCommand command,
            CancellationToken cancellationToken = default)
        {
            LastCommand = command;
            return Task.FromResult(new SendTemplateEmailResponse("Test", "message-id"));
        }
    }

    private sealed class FakeUserManager : UserManager<ApplicationUser>
    {
        private readonly ApplicationUser _user;
        private readonly bool _validResetToken;

        public FakeUserManager(
            ApplicationUser user,
            bool validResetToken)
            : base(
                new NoOpUserStore(),
                Microsoft.Extensions.Options.Options.Create(new IdentityOptions()),
                new PasswordHasher<ApplicationUser>(),
                Array.Empty<IUserValidator<ApplicationUser>>(),
                Array.Empty<IPasswordValidator<ApplicationUser>>(),
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                null!,
                Microsoft.Extensions.Logging.Abstractions.NullLogger<UserManager<ApplicationUser>>.Instance)
        {
            _user = user;
            _validResetToken = validResetToken;
        }

        public override Task<ApplicationUser?> FindByEmailAsync(string email)
        {
            return Task.FromResult<ApplicationUser?>(_user);
        }

        public override Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
        {
            return Task.FromResult(ResetToken);
        }

        public override Task<bool> VerifyUserTokenAsync(
            ApplicationUser user,
            string tokenProvider,
            string purpose,
            string token)
        {
            return Task.FromResult(_validResetToken);
        }
    }

    private sealed class NoOpUserStore : IUserStore<ApplicationUser>
    {
        public void Dispose()
        {
        }

        public Task<string> GetUserIdAsync(
            ApplicationUser user,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string?> GetUserNameAsync(
            ApplicationUser user,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetUserNameAsync(
            ApplicationUser user,
            string? userName,
            CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<string?> GetNormalizedUserNameAsync(
            ApplicationUser user,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task SetNormalizedUserNameAsync(
            ApplicationUser user,
            string? normalizedName,
            CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> CreateAsync(
            ApplicationUser user,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(
            ApplicationUser user,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(
            ApplicationUser user,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<ApplicationUser?> FindByIdAsync(
            string userId,
            CancellationToken cancellationToken)
        {
            return Task.FromResult<ApplicationUser?>(null);
        }

        public Task<ApplicationUser?> FindByNameAsync(
            string normalizedUserName,
            CancellationToken cancellationToken)
        {
            return Task.FromResult<ApplicationUser?>(null);
        }
    }
}
