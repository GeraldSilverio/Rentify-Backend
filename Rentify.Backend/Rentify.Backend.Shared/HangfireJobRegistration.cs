using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Rentify.Backend.Core.Application.Modules.Shared.Contracts;
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

            recurringJobManager.AddOrUpdate<IOutboxProcessor>(
                "process-outbox-messages",
                processor => processor.ProcessPendingMessagesAsync(CancellationToken.None),
                Cron.Minutely);

            return app;
        }
    }
}
