using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Rentify.Backend.Core.Application.Modules.Shared.Storage;
using System.Diagnostics;

namespace Rentify.Backend.Shared.Storage;

public sealed class CloudinaryFileStorageService : IFileStorageService
{
    private readonly Cloudinary _cloudinary;
    private readonly ILogger<CloudinaryFileStorageService> _logger;

    public CloudinaryFileStorageService(
        Cloudinary cloudinary,
        ILogger<CloudinaryFileStorageService> logger)
    {
        _cloudinary = cloudinary;
        _logger = logger;
    }

    public async Task<StoredFileResult> UploadAsync(
        IFormFile file,
        string folder,
        CancellationToken cancellationToken = default)
    {
        if (file is null || file.Length == 0)
            throw new InvalidOperationException("File is required.");

        Stopwatch stopwatch = Stopwatch.StartNew();
        _logger.LogInformation(
            "External provider {Provider} started operation {Operation} for file type {FileType}",
            "Cloudinary",
            "Upload",
            folder);

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
        {
            _logger.LogWarning(
                "External provider {Provider} operation {Operation} failed with status {StatusCode} for file type {FileType}",
                "Cloudinary",
                "Upload",
                (int)result.StatusCode,
                folder);
            throw new InvalidOperationException("Cloudinary file upload failed.");
        }

        if (result.SecureUrl is null || string.IsNullOrWhiteSpace(result.PublicId))
            throw new InvalidOperationException("Cloudinary did not return a valid file reference.");

        stopwatch.Stop();
        _logger.LogInformation(
            "External provider {Provider} completed operation {Operation} for file type {FileType} in {ElapsedMilliseconds} ms",
            "Cloudinary",
            "Upload",
            folder,
            stopwatch.ElapsedMilliseconds);

        return new StoredFileResult(result.SecureUrl.ToString(), result.PublicId);
    }

    public async Task DeleteAsync(string publicId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(publicId))
            return;

        Stopwatch stopwatch = Stopwatch.StartNew();

        var deletionParams = new DeletionParams(publicId)
        {
            ResourceType = ResourceType.Auto
        };

        DeletionResult result = await _cloudinary.DestroyAsync(deletionParams);
        stopwatch.Stop();

        _logger.LogInformation(
            "External provider {Provider} completed operation {Operation} with status {StatusCode} in {ElapsedMilliseconds} ms",
            "Cloudinary",
            "Delete",
            (int)result.StatusCode,
            stopwatch.ElapsedMilliseconds);
    }
}
