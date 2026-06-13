using Microsoft.AspNetCore.Http;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Services;

public interface IImageStorageService
{
    Task<StoredImageResult> UploadVehicleImageAsync(
        IFormFile image,
        CancellationToken cancellationToken = default);

    Task<StoredImageResult> UploadRentCarLogoAsync(
        IFormFile logo,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(string publicId, CancellationToken cancellationToken = default);
}
