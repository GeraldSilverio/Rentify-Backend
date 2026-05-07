using Microsoft.AspNetCore.Connections;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rentify.Backend.Core.Application.Helpers;
using Rentify.Backend.Core.Application.Interfaces.Repositories;
using Rentify.Backend.Infraestructure.Persistence.Context;
using Rentify.Backend.Infraestructure.Persistence.Repositories;

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

            SqlConnectionStringBuilder builder = new()
            {
                DataSource = $"{hostAddress},{portNumber}",
                InitialCatalog = dataBase,
                UserID = userDb,
                Password = passwordDb,
                TrustServerCertificate = true,
                ConnectTimeout = 30
            };

            services.AddDbContext<ApplicationContext>(options =>
                    options.UseNpgsql(builder.ConnectionString,
                        m => m.MigrationsAssembly(typeof(ApplicationContext).Assembly.FullName)),
                ServiceLifetime.Scoped);

            //Inyecciones de dependencias.
            services.AddTransient(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        }
    }
}