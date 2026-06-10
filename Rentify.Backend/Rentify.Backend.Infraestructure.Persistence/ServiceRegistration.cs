using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.RentCars.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Subscriptions.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Repositories;
using Rentify.Backend.Core.Application.Shared.Helpers;
using Rentify.Backend.Core.Application.Shared.UnitOfWork;
using Rentify.Backend.Infraestructure.Persistence.Emailing;
using Rentify.Backend.Infraestructure.Persistence.Context;
using Rentify.Backend.Infrastructure.Persistence.Repositories;

namespace Rentify.Backend.Infraestructure.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            string hostAddress = ReadFromConfiguration.GetValueFromConfig(configuration, "DB_HOST");
            string dataBase = ReadFromConfiguration.GetValueFromConfig(configuration, "DB_DATABASE_NAME");
            string userDb = ReadFromConfiguration.GetValueFromConfig(configuration, "DB_USER");
            string passwordDb = ReadFromConfiguration.GetValueFromConfig(configuration, "DB_PASSWORD");
            string portNumber = ReadFromConfiguration.GetValueFromConfig(configuration, "DB_PORT");

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = hostAddress,
                Port = int.Parse(portNumber),
                Database = dataBase,
                Username = userDb,
                Password = passwordDb
            };

            services.AddDbContext<RentifyContext>(options =>
                    options.UseNpgsql(builder.ConnectionString,
                        m => m.MigrationsAssembly(typeof(RentifyContext).Assembly.FullName)),
                ServiceLifetime.Scoped);

            //Inyecciones de dependencias.
            services.AddScoped<ITenantRepository, TenantRepository>();
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<IRentCarRepository, RentCarRepository>();
            services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
            services.AddScoped<ITenantEmailConfigurationRepository, TenantEmailConfigurationRepository>();
            services.AddScoped<IEmailProviderSender, ResendEmailProviderSender>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
