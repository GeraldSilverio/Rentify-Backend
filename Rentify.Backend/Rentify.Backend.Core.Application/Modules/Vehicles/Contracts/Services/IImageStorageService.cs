using Microsoft.AspNetCore.Http;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Services;

public interface IImageStorageService
{
    Task<StoredImageResult> SaveVehicleImageAsync(
        Guid vehicleId,
        IFormFile image,
        CancellationToken cancellationToken = default);

    Task<StoredImageResult> SaveRentCarLogoAsync(
        Guid rentCarId,
        IFormFile logo,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(string url, CancellationToken cancellationToken = default);
}
