using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Services;

namespace Rentify.Backend.Shared.Storage;

public sealed class CloudinaryImageStorageService : IImageStorageService
{
    private const string VehiclesFolder = "vehicles";
    private const string RentCarLogosFolder = "rent-cars/logos";
    private readonly Cloudinary _cloudinary;

    public CloudinaryImageStorageService(Cloudinary cloudinary)
    {
        _cloudinary = cloudinary;
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

        var deletionParams = new DeletionParams(publicId)
        {
            ResourceType = ResourceType.Image
        };

        await _cloudinary.DestroyAsync(deletionParams);
    }

    private async Task<StoredImageResult> UploadAsync(
        string folder,
        IFormFile image,
        CancellationToken cancellationToken)
    {
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
            throw new InvalidOperationException(result.Error.Message);

        if (result.SecureUrl is null || string.IsNullOrWhiteSpace(result.PublicId))
            throw new InvalidOperationException("Cloudinary did not return a valid image reference.");

        return new StoredImageResult(result.SecureUrl.ToString(), result.PublicId);
    }
}
