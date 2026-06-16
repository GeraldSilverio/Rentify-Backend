using CloudinaryDotNet;
using Microsoft.Extensions.DependencyInjection;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Services;
using Rentify.Backend.Core.Application.Shared.Storage;
using Rentify.Backend.Shared.Emailing;
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

        services.AddScoped<IEmailProviderSender, ResendEmailProviderSender>();
        services.AddScoped<IImageStorageService, CloudinaryImageStorageService>();
        services.AddScoped<IFileStorageService, CloudinaryFileStorageService>();

        return services;
    }
}
