using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Core.Application.Modules.Emails.Commands.SendTemplateEmail;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Emails.Dtos;
using Rentify.Backend.Core.Application.Modules.Emails.Implementations.Services;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Domain.Entities.Core;
using Rentify.Backend.Core.Domain.Enums;
using Rentify.Backend.Infraestructure.Persistence.Context;
using Rentify.Backend.Infraestructure.Persistence.Repositories;
using Serilog;
using Serilog.Core;

namespace Rentify.Backend.Logging.Tests;

public sealed class EmailServiceTemplateRenderingTests
{
    [Fact]
    public async Task ReplacesVariablesInSubjectHtmlAndTextWithoutLoggingBodies()
    {
        const string htmlTemplate = "<p>Hola {{CustomerFirstName}}, reserva {{ReservationCode}}</p>";
        const string textTemplate = "Hola {{CustomerFirstName}}, reserva {{ReservationCode}}";
        SystemEmailTemplate template = SystemEmailTemplate.Create(
            "RESERVATION_WEB",
            "Reservation web",
            "Solicitud {{ReservationCode}}",
            htmlTemplate,
            textTemplate,
            "tests");
        CapturingProvider provider = new();
        TestLogSink sink = new();
        using Logger serilogLogger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Sink(sink)
            .CreateLogger();
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            builder.AddSerilog(serilogLogger, dispose: false));
        EmailService service = new(
            new StubTemplateRepository(template),
            [provider],
            loggerFactory.CreateLogger<EmailService>());

        using EnvironmentVariablesScope environment = new();

        await service.SendEmailAsync(new SendTemplateEmailCommand(
            Guid.NewGuid(),
            "RESERVATION_WEB",
            "ana@example.test",
            new Dictionary<string, string>
            {
                ["CustomerFirstName"] = "Ana",
                ["ReservationCode"] = "RV-2026-0001"
            }));

        EmailProviderSendRequest request = Assert.Single(provider.Requests);
        Assert.Equal("Solicitud RV-2026-0001", request.Subject);
        Assert.Equal("<p>Hola Ana, reserva RV-2026-0001</p>", request.HtmlBody);
        Assert.Equal("Hola Ana, reserva RV-2026-0001", request.TextBody);
        Assert.DoesNotContain("{{", request.Subject);
        Assert.DoesNotContain("{{", request.HtmlBody);
        Assert.DoesNotContain("{{", request.TextBody);
        Assert.DoesNotContain(
            sink.Events,
            logEvent => logEvent.RenderMessage().Contains(
                htmlTemplate,
                StringComparison.Ordinal));
        Assert.DoesNotContain(
            sink.Events,
            logEvent => logEvent.RenderMessage().Contains(
                "ana@example.test",
                StringComparison.Ordinal));
    }

    [Fact]
    public async Task MissingTemplateFailsWithoutSending()
    {
        CapturingProvider provider = new();
        EmailService service = CreateService(
            new StubTemplateRepository(null),
            provider);

        await Assert.ThrowsAsync<ApiException>(() =>
            service.SendEmailAsync(new SendTemplateEmailCommand(
                Guid.NewGuid(),
                "RESERVATION_WEB",
                "ana@example.test",
                new Dictionary<string, string>())));

        Assert.Empty(provider.Requests);
    }

    [Fact]
    public async Task InactiveTemplateFailsWithoutSending()
    {
        DbContextOptions<RentifyContext> options =
            new DbContextOptionsBuilder<RentifyContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        await using RentifyContext context = new(options);
        SystemEmailTemplate template = SystemEmailTemplate.Create(
            "RESERVATION_WEB",
            "Reservation web",
            "Solicitud {{ReservationCode}}",
            "<p>{{ReservationCode}}</p>",
            "{{ReservationCode}}",
            "tests");
        template.IsActive = false;
        context.SystemEmailTemplates.Add(template);
        await context.SaveChangesAsync();
        CapturingProvider provider = new();
        EmailService service = CreateService(
            new SystemEmailTemplateRepository(context),
            provider);

        await Assert.ThrowsAsync<ApiException>(() =>
            service.SendEmailAsync(new SendTemplateEmailCommand(
                Guid.NewGuid(),
                "RESERVATION_WEB",
                "ana@example.test",
                new Dictionary<string, string>
                {
                    ["ReservationCode"] = "RV-2026-0001"
                })));

        Assert.Empty(provider.Requests);
    }

    [Fact]
    public async Task UnknownPlaceholderFailsBeforeProviderSend()
    {
        SystemEmailTemplate template = SystemEmailTemplate.Create(
            "RESERVATION_WEB",
            "Reservation web",
            "Solicitud {{ReservationCode}}",
            "<p>{{UnknownVariable}}</p>",
            "{{ReservationCode}}",
            "tests");
        CapturingProvider provider = new();
        EmailService service = CreateService(
            new StubTemplateRepository(template),
            provider);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SendEmailAsync(new SendTemplateEmailCommand(
                Guid.NewGuid(),
                "RESERVATION_WEB",
                "ana@example.test",
                new Dictionary<string, string>
                {
                    ["ReservationCode"] = "RV-2026-0001"
                })));

        Assert.Empty(provider.Requests);
    }

    [Fact]
    public async Task ProviderFailureIsRethrown()
    {
        SystemEmailTemplate template = SystemEmailTemplate.Create(
            "RESERVATION_TENANT",
            "Reservation tenant",
            "Reserva {{ReservationCode}}",
            "<p>{{ReservationCode}}</p>",
            "{{ReservationCode}}",
            "tests");
        CapturingProvider provider = new()
        {
            ExceptionToThrow = new InvalidOperationException("Resend unavailable")
        };
        EmailService service = CreateService(
            new StubTemplateRepository(template),
            provider);
        using EnvironmentVariablesScope environment = new();

        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SendEmailAsync(new SendTemplateEmailCommand(
                Guid.NewGuid(),
                "RESERVATION_TENANT",
                "ana@example.test",
                new Dictionary<string, string>
                {
                    ["ReservationCode"] = "RV-2026-0002"
                })));

        Assert.Equal("Resend unavailable", exception.Message);
        Assert.Single(provider.Requests);
    }

    private static EmailService CreateService(
        ISystemEmailTemplateRepository repository,
        IEmailProviderSender provider)
    {
        return new EmailService(
            repository,
            [provider],
            Microsoft.Extensions.Logging.Abstractions.NullLogger<EmailService>.Instance);
    }

    private sealed class CapturingProvider : IEmailProviderSender
    {
        public EmailProviderType Provider => EmailProviderType.Resend;
        public List<EmailProviderSendRequest> Requests { get; } = [];
        public Exception? ExceptionToThrow { get; init; }

        public Task<string> SendAsync(
            EmailProviderSendRequest request,
            CancellationToken cancellationToken = default)
        {
            Requests.Add(request);

            if (ExceptionToThrow is not null)
            {
                throw ExceptionToThrow;
            }

            return Task.FromResult("message-id");
        }
    }

    private sealed class StubTemplateRepository : ISystemEmailTemplateRepository
    {
        private readonly SystemEmailTemplate? _template;

        public StubTemplateRepository(SystemEmailTemplate? template)
        {
            _template = template;
        }

        public Task<SystemEmailTemplate?> GetByCodeAsync(Guid? tenantId, string code, CancellationToken cancellationToken = default) => Task.FromResult(_template);
        public Task AddAsync(SystemEmailTemplate emailTemplate, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IReadOnlyList<SystemEmailTemplate>> ListAsync(Guid? tenantId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<SystemEmailTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<bool> ExistsByCodeAsync(Guid? tenantId, string code, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }

    private sealed class EnvironmentVariablesScope : IDisposable
    {
        private readonly Dictionary<string, string?> _originalValues = new();

        public EnvironmentVariablesScope()
        {
            Set("RESEND_API_KEY", "test-api-key");
            Set("EMAIL_FROM", "sender@example.test");
            Set("EMAIL_FROM_NAME", "Rentify Tests");
        }

        public void Dispose()
        {
            foreach ((string key, string? value) in _originalValues)
            {
                Environment.SetEnvironmentVariable(key, value);
            }
        }

        private void Set(string key, string value)
        {
            _originalValues[key] = Environment.GetEnvironmentVariable(key);
            Environment.SetEnvironmentVariable(key, value);
        }
    }
}
