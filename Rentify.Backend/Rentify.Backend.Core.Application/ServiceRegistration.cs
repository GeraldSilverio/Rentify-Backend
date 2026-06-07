using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Rentify.Backend.Core.Application.Common.Behaviors;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Tenants.Implementations.Services;
using System.Reflection;
using Rentify.Backend.Core.Application.Modules.RentCars.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.RentCars.Implementations.Services;
using Rentify.Backend.Core.Application.Modules.Subscriptions.Contracts.Services;

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

        var assembly = Assembly.GetExecutingAssembly();

        // MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
        });

        // FluentValidation
        services.AddValidatorsFromAssembly(assembly);

        // Pipeline Behaviors
        services.AddScoped(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));

        services.AddScoped<IRentCarService, RentCarService>();
        // services.AddScoped<ISubscriptionService,SubscriptionService>();
        services.AddScoped<ITenantService,TenantService>();

    }
}