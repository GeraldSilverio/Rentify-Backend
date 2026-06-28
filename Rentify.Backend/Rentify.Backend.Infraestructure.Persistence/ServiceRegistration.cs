using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Rentify.Backend.Core.Application.Modules.Customers.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Dashboard.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Payments.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Reservations.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Subscriptions.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Infraestructure.Persistence.Context;
using Rentify.Backend.Infrastructure.Persistence.Repositories;
using Rentify.Backend.Infraestructure.Persistence.Repositories;
using Rentify.Backend.Infraestructure.Persistence.Repositories.Core;
using Rentify.Backend.Infraestructure.Persistence.Repositories.Vehicules;
using Rentify.Backend.Core.Application.Modules.Shared.Helpers;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;

namespace Rentify.Backend.Infraestructure.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            string hostAddress = ReadFromConfiguration.GetValueFromConfig("DB_HOST");
            string dataBase = ReadFromConfiguration.GetValueFromConfig("DB_DATABASE_NAME");
            string userDb = ReadFromConfiguration.GetValueFromConfig("DB_USER");
            string passwordDb = ReadFromConfiguration.GetValueFromConfig("DB_PASSWORD");
            string portNumber = ReadFromConfiguration.GetValueFromConfig("DB_PORT");

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
            services.AddScoped<ITenantReadRepository, TenantReadRepository>();
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<ISystemEmailTemplateRepository, SystemEmailTemplateRepository>();
            services.AddScoped<ITenantEmailConfigurationRepository, TenantEmailConfigurationRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ISubscriptionPlanRepository, SubscriptionPlanRepository>();
            services.AddScoped<IVehicleRepository, VehicleRepository>();
            services.AddScoped<IVehicleCatalogRepository, VehicleCatalogRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IReservationRepository, ReservationRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IDashboardRepository, DashboardRepository>();
            services.AddScoped<ITenantSettingRepository, TenantSettingRepository>();
            services.AddScoped<IPaymentPolicyRepository, PaymentPolicyRepository>();

        }
    }
}
