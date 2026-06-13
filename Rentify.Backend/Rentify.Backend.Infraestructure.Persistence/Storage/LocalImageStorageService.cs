using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Services;

namespace Rentify.Backend.Infraestructure.Persistence.Storage;

public sealed class LocalImageStorageService : IImageStorageService
{
    private const string VehicleImagesDirectory = "uploads/vehicles";
    private const string RentCarLogosDirectory = "uploads/rent-cars/logos";
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
        return await SaveAsync(VehicleImagesDirectory, vehicleId, image, cancellationToken);
    }

    public async Task<StoredImageResult> SaveRentCarLogoAsync(
        Guid rentCarId,
        IFormFile logo,
        CancellationToken cancellationToken = default)
    {
        return await SaveAsync(RentCarLogosDirectory, rentCarId, logo, cancellationToken);
    }

    public Task DeleteAsync(string url, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url) || !IsKnownUploadUrl(url))
            return Task.CompletedTask;

        string webRootPath = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
        string relativePath = url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        string physicalPath = Path.GetFullPath(Path.Combine(webRootPath, relativePath));
        string allowedRoot = Path.GetFullPath(Path.Combine(webRootPath, "uploads"));

        if (!physicalPath.StartsWith(allowedRoot, StringComparison.OrdinalIgnoreCase))
            return Task.CompletedTask;

        if (File.Exists(physicalPath))
            File.Delete(physicalPath);

        return Task.CompletedTask;
    }

    private async Task<StoredImageResult> SaveAsync(
        string baseDirectory,
        Guid ownerId,
        IFormFile image,
        CancellationToken cancellationToken)
    {
        string extension = Path.GetExtension(image.FileName);
        string fileName = $"{Guid.NewGuid():N}{extension}";
        string relativeDirectory = $"{baseDirectory}/{ownerId:N}";
        string webRootPath = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
        string physicalDirectory = Path.Combine(webRootPath, relativeDirectory.Replace('/', Path.DirectorySeparatorChar));

        Directory.CreateDirectory(physicalDirectory);

        string physicalPath = Path.Combine(physicalDirectory, fileName);

        await using FileStream stream = File.Create(physicalPath);
        await image.CopyToAsync(stream, cancellationToken);

        string url = $"/{relativeDirectory}/{fileName}";

        return new StoredImageResult(url, fileName, image.ContentType, image.Length);
    }

    private static bool IsKnownUploadUrl(string url)
    {
        return url.StartsWith($"/{VehicleImagesDirectory}", StringComparison.OrdinalIgnoreCase)
               || url.StartsWith($"/{RentCarLogosDirectory}", StringComparison.OrdinalIgnoreCase);
    }
}
