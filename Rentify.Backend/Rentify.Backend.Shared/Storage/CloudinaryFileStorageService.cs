using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Shared.Storage;

namespace Rentify.Backend.Shared.Storage;

public sealed class CloudinaryFileStorageService : IFileStorageService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryFileStorageService(Cloudinary cloudinary)
    {
        _cloudinary = cloudinary;
    }

    public async Task<StoredFileResult> UploadAsync(
        IFormFile file,
        string folder,
        CancellationToken cancellationToken = default)
    {
        if (file is null || file.Length == 0)
            throw new InvalidOperationException("File is required.");

        await using Stream stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = folder,
            UseFilename = false,
            UniqueFilename = true,
            Overwrite = false
        };

        ImageUploadResult result = await _cloudinary.UploadAsync(uploadParams, cancellationToken);

        if (result.Error is not null)
            throw new InvalidOperationException(result.Error.Message);

        if (result.SecureUrl is null || string.IsNullOrWhiteSpace(result.PublicId))
            throw new InvalidOperationException("Cloudinary did not return a valid file reference.");

        return new StoredFileResult(result.SecureUrl.ToString(), result.PublicId);
    }

    public async Task DeleteAsync(string publicId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(publicId))
            return;

        var deletionParams = new DeletionParams(publicId)
        {
            ResourceType = ResourceType.Auto
        };

        await _cloudinary.DestroyAsync(deletionParams);
    }
}
