using Microsoft.Extensions.Logging;
using Rentify.Backend.Core.Application.Modules.Shared.Contracts;
using Rentify.Backend.Infraestructure.Shared;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Rentify.Backend.Logging.Tests;

public sealed class OutboxProcessingJobTests
{
    [Fact]
    public async Task FailedJobLogsExceptionAndRethrows()
    {
        TestLogSink sink = new();
        using Logger serilogLogger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Sink(sink)
            .CreateLogger();
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            builder.AddSerilog(serilogLogger, dispose: false));

        OutboxProcessingJob job = new(
            new ThrowingOutboxProcessor(),
            loggerFactory.CreateLogger<OutboxProcessingJob>());

        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            job.ProcessPendingMessagesAsync(null));

        Assert.Equal("Synthetic outbox failure", exception.Message);
        LogEvent failure = Assert.Single(sink.Events, logEvent =>
            logEvent.MessageTemplate.Text ==
            "Hangfire job {JobType} failed with Job {JobId} in {ElapsedMilliseconds} ms");
        Assert.Equal(LogEventLevel.Error, failure.Level);
        Assert.IsType<InvalidOperationException>(failure.Exception);
    }

    private sealed class ThrowingOutboxProcessor : IOutboxProcessor
    {
        public Task ProcessPendingMessagesAsync(CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("Synthetic outbox failure");
        }
    }
}
