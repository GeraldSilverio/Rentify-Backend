using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.PostgreSql;
using Hangfire.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rentify.Backend.Core.Application.Modules.Shared.Contracts;

namespace Rentify.Backend.Infraestructure.Shared
{
    public static class HangfireJobRegistration
    {
        private const string ProcessOutboxMessagesJobId =
            "process-outbox-messages";

        public static WebApplication UseRentifyHangfireJobs(
            this WebApplication app)
        {
            using IServiceScope scope = app.Services.CreateScope();

            IRecurringJobManager recurringJobManager =
                scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

            ILogger logger = scope.ServiceProvider
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger(nameof(HangfireJobRegistration));

            try
            {
                recurringJobManager.AddOrUpdate<OutboxProcessingJob>(
                    ProcessOutboxMessagesJobId,
                    job => job.ProcessPendingMessagesAsync(null),
                    Cron.Minutely);

                logger.LogInformation(
                    "Hangfire recurring job {RecurringJobId} registered successfully",
                    ProcessOutboxMessagesJobId);
            }
            catch (PostgreSqlDistributedLockException exception)
            {
                /*
                 * Otra instancia del backend está registrando o actualizando
                 * el mismo recurring job.
                 *
                 * No se detiene la aplicación porque el job probablemente
                 * ya se encuentra registrado en Hangfire.
                 */
                logger.LogWarning(
                    exception,
                    "Could not register Hangfire recurring job {RecurringJobId} " +
                    "because another application instance holds the distributed lock. " +
                    "Rentify Backend will continue starting",
                    ProcessOutboxMessagesJobId);
            }
            catch (Exception exception)
            {
                /*
                 * Se registra cualquier error inesperado, pero no se detiene
                 * toda la API por un fallo al registrar un recurring job.
                 */
                logger.LogError(
                    exception,
                    "An unexpected error occurred while registering Hangfire " +
                    "recurring job {RecurringJobId}. Rentify Backend will continue starting",
                    ProcessOutboxMessagesJobId);
            }

            return app;
        }
    }

    public sealed class OutboxProcessingJob
    {
        private readonly IOutboxProcessor _outboxProcessor;
        private readonly ILogger<OutboxProcessingJob> _logger;

        public OutboxProcessingJob(
            IOutboxProcessor outboxProcessor,
            ILogger<OutboxProcessingJob> logger)
        {
            _outboxProcessor = outboxProcessor;
            _logger = logger;
        }

        [DisableConcurrentExecution(timeoutInSeconds: 300)]
        public async Task ProcessPendingMessagesAsync(
            PerformContext? context)
        {
            string? jobId = context?.BackgroundJob?.Id;

            Stopwatch stopwatch = Stopwatch.StartNew();

            using IDisposable? loggingScope =
                _logger.BeginScope(
                    new Dictionary<string, object?>
                    {
                        ["JobId"] = jobId,
                        ["JobType"] = nameof(OutboxProcessingJob)
                    });

            _logger.LogInformation(
                "Hangfire job {JobType} started with JobId {JobId}",
                nameof(OutboxProcessingJob),
                jobId);

            try
            {
                await _outboxProcessor.ProcessPendingMessagesAsync();

                _logger.LogInformation(
                    "Hangfire job {JobType} completed successfully with " +
                    "JobId {JobId} in {ElapsedMilliseconds} ms",
                    nameof(OutboxProcessingJob),
                    jobId,
                    stopwatch.ElapsedMilliseconds);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Hangfire job {JobType} failed with JobId {JobId} " +
                    "after {ElapsedMilliseconds} ms",
                    nameof(OutboxProcessingJob),
                    jobId,
                    stopwatch.ElapsedMilliseconds);

                throw;
            }
            finally
            {
                stopwatch.Stop();
            }
        }
    }
}