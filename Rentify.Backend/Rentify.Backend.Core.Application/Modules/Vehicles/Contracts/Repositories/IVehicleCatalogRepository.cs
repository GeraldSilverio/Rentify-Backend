using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;

public interface IVehicleCatalogRepository
{
    Task<IReadOnlyCollection<BrandResponse>> GetBrandsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ModelResponse>> GetModelsByBrandAsync(Guid brandId, CancellationToken cancellationToken = default);
    Task<bool> BrandExistsAsync(Guid brandId, CancellationToken cancellationToken = default);
}
