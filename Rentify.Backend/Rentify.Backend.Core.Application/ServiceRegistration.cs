using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Services;
using System.Reflection;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Emails.Implementations.Services;
using Rentify.Backend.Core.Application.Modules.Customers.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Customers.Implementations.Services;
using Rentify.Backend.Core.Application.Modules.Payments.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Payments.Implementations.Services;
using Rentify.Backend.Core.Application.Modules.Reservations.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Reservations.Implementations.Services;
using Rentify.Backend.Core.Application.Modules.Subscriptions.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Subscriptions.Implementations;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Vehicles.Implementations.Services;
using Rentify.Backend.Core.Application.Modules.Shared.Behaviors;
using Rentify.Backend.Core.Application.Modules.Tenants.Services;
using Rentify.Backend.Core.Application.Modules.Core.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Core.Services;

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

        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<IVehicleService, VehicleService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<ITenantService,TenantService>();
        services.AddScoped<ITenantSettingService,TenantSettingService>();
        services.AddScoped<IPaymentPolicyService,PaymentPolicyService>();
    }
}
