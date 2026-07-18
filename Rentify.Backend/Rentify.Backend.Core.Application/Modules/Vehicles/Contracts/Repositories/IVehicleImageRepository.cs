using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.SetPrimaryVehicleImage;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UploadVehicleImage;
using Rentify.Backend.Core.Domain.Entities.Vehicles;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories
{
    public interface IVehicleImageRepository
    {
        Task<int> GetTotalImagesByVehicleAsync(Guid tenantId, Guid vehicleId, CancellationToken cancellationToken = default);
        Task AddImagesAsync(IEnumerable<VehicleImage> images, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<VehicleImageResponse>> GetImagesAsync(Guid tenantId, Guid vehicleId, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<VehicleImageResponse>> UploadImagesAsync(UploadVehicleImageCommand command, CancellationToken cancellationToken = default);
        Task DeleteImageAsync(Guid tenantId, Guid vehicleId, Guid imageId, string modifiedBy, CancellationToken cancellationToken = default);
        Task SetPrimaryImageAsync(SetPrimaryVehicleImageCommand command, CancellationToken cancellationToken = default);
        Task<VehicleImageResponse?> GetImageByIdAsync(Guid tenantId, Guid vehicleId, Guid imageId, CancellationToken cancellationToken = default);
    }
}
