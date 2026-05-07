using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Rentify.Backend.Core.Application
{
    /// <summary>
    /// Static class for registering application layer services.
    /// </summary>
    public static class ServiceRegistration
    {
        /// <summary>
        /// Registers application layer services such as AutoMapper and MediatR.
        /// </summary>
        /// <param name="services">The collection of services to add to.</param>
        public static void AddApplicationLayer(this IServiceCollection services)
        {
            
        }
    }
}