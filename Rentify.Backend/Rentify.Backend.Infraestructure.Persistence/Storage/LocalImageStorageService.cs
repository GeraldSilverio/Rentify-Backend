using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Services;

namespace Rentify.Backend.Infraestructure.Persistence.Storage;

public sealed class LocalImageStorageService : IImageStorageService
{
    private const string VehicleImagesDirectory = "uploads/vehicles";
    private readonly IWebHostEnvironment _environment;

    public LocalImageStorageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<StoredImageResult> SaveVehicleImageAsync(
        Guid vehicleId,
        IFormFile image,
        CancellationToken cancellationToken = default)
    {
        string extension = Path.GetExtension(image.FileName);
        string fileName = $"{Guid.NewGuid():N}{extension}";
        string relativeDirectory = $"{VehicleImagesDirectory}/{vehicleId:N}";
        string webRootPath = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
        string physicalDirectory = Path.Combine(webRootPath, relativeDirectory.Replace('/', Path.DirectorySeparatorChar));

        Directory.CreateDirectory(physicalDirectory);

        string physicalPath = Path.Combine(physicalDirectory, fileName);

        await using FileStream stream = File.Create(physicalPath);
        await image.CopyToAsync(stream, cancellationToken);

        string url = $"/{relativeDirectory}/{fileName}";

        return new StoredImageResult(url, fileName, image.ContentType, image.Length);
    }

    public Task DeleteAsync(string url, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url) || !url.StartsWith($"/{VehicleImagesDirectory}", StringComparison.OrdinalIgnoreCase))
            return Task.CompletedTask;

        string webRootPath = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
        string relativePath = url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        string physicalPath = Path.GetFullPath(Path.Combine(webRootPath, relativePath));
        string allowedRoot = Path.GetFullPath(Path.Combine(webRootPath, VehicleImagesDirectory.Replace('/', Path.DirectorySeparatorChar)));

        if (!physicalPath.StartsWith(allowedRoot, StringComparison.OrdinalIgnoreCase))
            return Task.CompletedTask;

        if (File.Exists(physicalPath))
            File.Delete(physicalPath);

        return Task.CompletedTask;
    }
}
