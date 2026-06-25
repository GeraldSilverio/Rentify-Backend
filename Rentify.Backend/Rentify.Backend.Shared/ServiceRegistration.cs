using CloudinaryDotNet;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Services;
using Rentify.Backend.Core.Application.Shared.Contracts;
using Rentify.Backend.Core.Application.Shared.Helpers;
using Rentify.Backend.Core.Application.Shared.Storage;
using Rentify.Backend.Infraestructure.Shared.Emailing;
using Rentify.Backend.Infraestructure.Shared.Services;
using Rentify.Backend.Infraestructure.Shared.Services.OutBox;
using Rentify.Backend.Shared.Storage;

namespace Rentify.Backend.Shared;

public static class ServiceRegistration
{
    public static IServiceCollection AddSharedServices(this IServiceCollection services)
    {
        CloudinarySettings cloudinarySettings = CloudinarySettings.FromEnvironment();

        services.AddSingleton(new Cloudinary(new Account(
            cloudinarySettings.CloudName,
            cloudinarySettings.ApiKey,
            cloudinarySettings.ApiSecret)));


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


        services.AddHangfire(configuration =>
        {
            configuration.UsePostgreSqlStorage(options =>
            {
                options.UseNpgsqlConnection(builder.ConnectionString);
            });
        });

        services.AddHangfireServer();

        services.AddScoped<IEmailProviderSender, ResendEmailProviderSender>();
        services.AddScoped<IImageStorageService, CloudinaryImageStorageService>();
        services.AddScoped<IFileStorageService, CloudinaryFileStorageService>();
        services.AddScoped<IOutboxService, OutboxService>();
        services.AddScoped<IOutboxProcessor, OutboxProcessor>();
        services.AddScoped<IOutboxMessageHandler, TenantRegisteredOutboxHandler>();

        return services;
    }

}
