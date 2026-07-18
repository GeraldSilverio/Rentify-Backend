using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UploadVehicleImage;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Services;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicleImages;

public sealed class GetVehicleImagesHandler
    : IRequestHandler<GetVehicleImagesQuery, ResultReponse<IReadOnlyCollection<VehicleImageResponse>>>
{
    private readonly IVehicleImageRepository _vehicleImageRepository;

    public GetVehicleImagesHandler(IVehicleImageRepository vehicleImageRepository)
    {
        _vehicleImageRepository = vehicleImageRepository;
    }

    public async Task<ResultReponse<IReadOnlyCollection<VehicleImageResponse>>> Handle(
        GetVehicleImagesQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<VehicleImageResponse> images = await _vehicleImageRepository.GetImagesAsync(
            request.TenantId,
            request.VehicleId,
            cancellationToken);

        return ResultReponse<IReadOnlyCollection<VehicleImageResponse>>.Success(images);
    }
}
