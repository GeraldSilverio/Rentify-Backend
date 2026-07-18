using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.SetPrimaryVehicleImage;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UploadVehicleImage;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Domain.Entities.Vehicles;
using Rentify.Backend.Infraestructure.Persistence.Context;
using static System.Net.Mime.MediaTypeNames;

namespace Rentify.Backend.Infraestructure.Persistence.Repositories.Vehicules
{
    public class VehicleImageRepository(RentifyContext context) : IVehicleImageRepository
    {
        public async Task AddImagesAsync(IEnumerable<VehicleImage> images, CancellationToken cancellationToken = default)
        {
            await context.VehicleImages.AddRangeAsync(images, cancellationToken);
        }

        public async Task<VehicleImageResponse?> GetImageByIdAsync(Guid tenantId, Guid vehicleId, Guid imageId, CancellationToken cancellationToken = default)
        {
            return await context.VehicleImages
                .Where(x => x.TenantId == tenantId && x.VehicleId == vehicleId && x.Id == imageId && x.IsActive && !x.IsDeleted)
                .Select(x => new VehicleImageResponse(
                    x.Id,
                    x.Url,
                    x.PublicId,
                    x.IsPrimary,
                    x.CreatedDate
                )).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task DeleteImageAsync(Guid tenantId, Guid vehicleId, Guid imageId, string modifiedBy, CancellationToken cancellationToken = default)
        {
            VehicleImage image = await context.VehicleImages.FirstAsync(x => x.TenantId == tenantId
               && x.VehicleId == vehicleId
               && x.Id == imageId
               && x.IsActive
               && !x.IsDeleted, cancellationToken);


            if (image.IsPrimary)
            {
                VehicleImage? nextPrimary = context.VehicleImages
                    .Where(x => !x.IsDeleted && x.VehicleId == vehicleId)
                    .OrderBy(x => x.CreatedDate)
                    .FirstOrDefault();

                nextPrimary?.MarkAsPrimary(modifiedBy);
            }


            context.VehicleImages.Remove(image);
        }

        public async Task<IReadOnlyCollection<VehicleImageResponse>> GetImagesAsync(Guid tenantId, Guid vehicleId, CancellationToken cancellationToken = default)
        {
            return await context.VehicleImages.Where(x => x.TenantId == tenantId && x.VehicleId == vehicleId && x.IsActive && !x.IsDeleted)
                .Select(x => new VehicleImageResponse(

                    x.Id,
                    x.Url,
                    x.PublicId,
                    x.IsPrimary,
                    x.CreatedDate
                ))
                .ToListAsync(cancellationToken);
        }

        public Task<int> GetTotalImagesByVehicleAsync(Guid tenantId, Guid vehicleId, CancellationToken cancellationToken = default)
        {
            return context.VehicleImages
                .Where(x => x.IsActive && !x.IsDeleted && x.VehicleId == vehicleId && x.TenantId == tenantId)
                .CountAsync(cancellationToken);
        }

        public Task SetPrimaryImageAsync(SetPrimaryVehicleImageCommand command, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<VehicleImageResponse>> UploadImagesAsync(UploadVehicleImageCommand command, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
