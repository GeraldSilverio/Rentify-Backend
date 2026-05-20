using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Rentify.Backend.Core.Application.Contracts.Services;
using Rentify.Backend.Core.Application.Services;
using System.Reflection;

namespace Rentify.Backend.Core.Application;

/// <summary>
/// Static class for registering application layer services.
/// </summary>
public static class ServiceRegistration
{
    /// <summary>
    /// Registers application layer services such as FluentValidation and RentCar services.
    /// </summary>
    /// <param name="services">The collection of services to add to.</param>
    public static void AddApplicationLayer(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddScoped<IRentCarService, RentCarService>();
    }
}