using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rentify.Backend.Core.Application.Interfaces.Repositories;
using Rentify.Backend.Infraestructure.Persistence.Context;
using Rentify.Backend.Infraestructure.Persistence.Repositories;

namespace Rentify.Backend.Infraestructure.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                        m => m.MigrationsAssembly(typeof(ApplicationContext).Assembly.FullName)),
                ServiceLifetime.Scoped);

            //Inyecciones de dependencias.
            services.AddTransient(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        }
    }
}