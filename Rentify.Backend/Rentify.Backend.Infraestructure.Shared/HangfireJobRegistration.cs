using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Hangfire.Server;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Backend.Infraestructure.Shared
{
    public static class HangfireJobRegistration
    {
        public static WebApplication UseRentifyHangfireJobs(this WebApplication app)
        {
            using IServiceScope scope = app.Services.CreateScope();

            IRecurringJobManager recurringJobManager =
                scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

            recurringJobManager.AddOrUpdate<OutboxProcessingJob>(
                "process-outbox-messages",
                job => job.ProcessPendingMessagesAsync(null),
                Cron.Minutely);

            return app;
        }
    }

    public sealed class OutboxProcessingJob
    {
        private readonly Rentify.Backend.Core.Application.Modules.Shared.Contracts.IOutboxProcessor _outboxProcessor;
        private readonly ILogger<OutboxProcessingJob> _logger;

        public OutboxProcessingJob(
            Rentify.Backend.Core.Application.Modules.Shared.Contracts.IOutboxProcessor outboxProcessor,
            ILogger<OutboxProcessingJob> logger)
        {
            _outboxProcessor = outboxProcessor;
            _logger = logger;
        }

        [DisableConcurrentExecution(timeoutInSeconds: 300)]
        public async Task ProcessPendingMessagesAsync(PerformContext? context)
        {
            string? jobId = context?.BackgroundJob?.Id;
            Stopwatch stopwatch = Stopwatch.StartNew();

            _logger.LogInformation(
                "Hangfire job {JobType} started with Job {JobId}",
                nameof(OutboxProcessingJob),
                jobId);

            try
            {
                await _outboxProcessor.ProcessPendingMessagesAsync();
                stopwatch.Stop();

                _logger.LogInformation(
                    "Hangfire job {JobType} completed with Job {JobId} in {ElapsedMilliseconds} ms",
                    nameof(OutboxProcessingJob),
                    jobId,
                    stopwatch.ElapsedMilliseconds);
            }
            catch (Exception exception)
            {
                stopwatch.Stop();

                _logger.LogError(
                    exception,
                    "Hangfire job {JobType} failed with Job {JobId} in {ElapsedMilliseconds} ms",
                    nameof(OutboxProcessingJob),
                    jobId,
                    stopwatch.ElapsedMilliseconds);

                throw;
            }
        }
    }
}
