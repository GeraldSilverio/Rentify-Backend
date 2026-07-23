using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Backend.Infrastructure.Shared
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
                job => job.ProcessPendingMessagesAsync(),
                Cron.Minutely);

            return app;
        }
    }

    public sealed class OutboxProcessingJob
    {
        private readonly Rentify.Backend.Core.Application.Modules.Shared.Contracts.IOutboxProcessor _outboxProcessor;

        public OutboxProcessingJob(Rentify.Backend.Core.Application.Modules.Shared.Contracts.IOutboxProcessor outboxProcessor)
        {
            _outboxProcessor = outboxProcessor;
        }

        [DisableConcurrentExecution(timeoutInSeconds: 300)]
        public Task ProcessPendingMessagesAsync()
        {
            return _outboxProcessor.ProcessPendingMessagesAsync();
        }
    }
}
