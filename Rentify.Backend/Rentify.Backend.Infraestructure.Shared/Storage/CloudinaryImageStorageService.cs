using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Services;
using System.Diagnostics;

namespace Rentify.Backend.Shared.Storage;

public sealed class CloudinaryImageStorageService : IImageStorageService
{
    private const string VehiclesFolder = "vehicles";
    private const string RentCarLogosFolder = "rent-cars/logos";
    private readonly Cloudinary _cloudinary;
    private readonly ILogger<CloudinaryImageStorageService> _logger;

    public CloudinaryImageStorageService(
        Cloudinary cloudinary,
        ILogger<CloudinaryImageStorageService> logger)
    {
        _cloudinary = cloudinary;
        _logger = logger;
    }

    public async Task<StoredImageResult> UploadVehicleImageAsync(
        IFormFile image,
        CancellationToken cancellationToken = default)
    {
        return await UploadAsync(VehiclesFolder, image, cancellationToken);
    }

    public async Task<StoredImageResult> UploadRentCarLogoAsync(
        IFormFile logo,
        CancellationToken cancellationToken = default)
    {
        return await UploadAsync(RentCarLogosFolder, logo, cancellationToken);
    }

    public async Task DeleteAsync(string publicId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(publicId))
            return;

        Stopwatch stopwatch = Stopwatch.StartNew();

        var deletionParams = new DeletionParams(publicId)
        {
            ResourceType = ResourceType.Image
        };

        DeletionResult result = await _cloudinary.DestroyAsync(deletionParams);
        stopwatch.Stop();

        _logger.LogInformation(
            "External provider {Provider} completed operation {Operation} with status {StatusCode} in {ElapsedMilliseconds} ms",
            "Cloudinary",
            "DeleteImage",
            (int)result.StatusCode,
            stopwatch.ElapsedMilliseconds);
    }

    private async Task<StoredImageResult> UploadAsync(
        string folder,
        IFormFile image,
        CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        _logger.LogInformation(
            "External provider {Provider} started operation {Operation} for image type {ImageType}",
            "Cloudinary",
            "UploadImage",
            folder);

        await using Stream stream = image.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(image.FileName, stream),
            Folder = folder,
            UseFilename = false,
            UniqueFilename = true,
            Overwrite = false
        };

        ImageUploadResult result = await _cloudinary.UploadAsync(uploadParams, cancellationToken);

        if (result.Error is not null)
        {
            _logger.LogWarning(
                "External provider {Provider} operation {Operation} failed with status {StatusCode} for image type {ImageType}",
                "Cloudinary",
                "UploadImage",
                (int)result.StatusCode,
                folder);
            throw new InvalidOperationException("Cloudinary image upload failed.");
        }

        if (result.SecureUrl is null || string.IsNullOrWhiteSpace(result.PublicId))
            throw new InvalidOperationException("Cloudinary did not return a valid image reference.");

        stopwatch.Stop();
        _logger.LogInformation(
            "External provider {Provider} completed operation {Operation} for image type {ImageType} in {ElapsedMilliseconds} ms",
            "Cloudinary",
            "UploadImage",
            folder,
            stopwatch.ElapsedMilliseconds);

        return new StoredImageResult(result.SecureUrl.ToString(), result.PublicId);
    }
}
