using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using Rentify.Backend.Core.Application.Modules.Shared.Behaviors;
using Rentify.Backend.Core.Application.Modules.Shared.Context;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Serilog;
using Serilog.Core;

namespace Rentify.Backend.Logging.Tests;

public sealed class LoggingBehaviorTests
{
    [Fact]
    public async Task CommandLogsMetadataWithoutSerializingSensitiveRequest()
    {
        TestLogSink sink = new();
        using Logger serilogLogger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Sink(sink)
            .CreateLogger();
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            builder.AddSerilog(serilogLogger, dispose: false));

        LoggingBehavior<SensitiveCommand, ResultReponse<bool>> behavior = new(
            loggerFactory.CreateLogger<LoggingBehavior<SensitiveCommand, ResultReponse<bool>>>(),
            new FakeCurrentRequestContext());

        using Activity activity = new("logging-behavior-test");
        activity.Start();
        activity.SetTag("CorrelationId", "behavior-correlation");

        SensitiveCommand request = new(
            "password-do-not-log",
            "token-do-not-log",
            "private.user@example.test");

        ResultReponse<bool> response = await behavior.Handle(
            request,
            _ => Task.FromResult(ResultReponse<bool>.Success(true)),
            CancellationToken.None);

        string logs = sink.RenderAll();
        Assert.True(response.IsSuccess);
        Assert.Contains(nameof(SensitiveCommand), logs, StringComparison.Ordinal);
        Assert.Contains("behavior-correlation", logs, StringComparison.Ordinal);
        Assert.Contains(TestIdentifiers.TenantId.ToString(), logs, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(TestIdentifiers.UserId.ToString(), logs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(request.Password, logs, StringComparison.Ordinal);
        Assert.DoesNotContain(request.Token, logs, StringComparison.Ordinal);
        Assert.DoesNotContain(request.Email, logs, StringComparison.Ordinal);
    }

    private sealed record SensitiveCommand(
        string Password,
        string Token,
        string Email) : IRequest<ResultReponse<bool>>;

    private sealed class FakeCurrentRequestContext : ICurrentRequestContext
    {
        public bool IsAuthenticated => true;
        public Guid UserId => TestIdentifiers.UserId;
        public Guid TenantId => TestIdentifiers.TenantId;
        public bool HasTenant => true;
        public string? UserName => null;
        public string? Email => null;
        public IReadOnlyCollection<string> Roles => Array.Empty<string>();
        public bool IsSuperAdmin => false;
        public string ModifiedBy => UserId.ToString();

        public bool TryGetTenantId(out Guid tenantId)
        {
            tenantId = TenantId;
            return true;
        }
    }
}
