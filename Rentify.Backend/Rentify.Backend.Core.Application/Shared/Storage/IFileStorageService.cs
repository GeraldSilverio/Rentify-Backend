using Microsoft.AspNetCore.Http;

namespace Rentify.Backend.Core.Application.Shared.Storage;

public interface IFileStorageService
{
    Task<StoredFileResult> UploadAsync(
        IFormFile file,
        string folder,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(string publicId, CancellationToken cancellationToken = default);
}
