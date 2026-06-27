using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Services;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UploadVehicleImage;

public sealed class UploadVehicleImageHandler : IRequestHandler<UploadVehicleImageCommand, ResultReponse<IReadOnlyCollection<VehicleImageResponse>>>
{
    private readonly IVehicleService _vehicleService;

    public UploadVehicleImageHandler(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    public async Task<ResultReponse<IReadOnlyCollection<VehicleImageResponse>>> Handle(
        UploadVehicleImageCommand request,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<VehicleImageResponse> images = await _vehicleService.UploadImagesAsync(request, cancellationToken);

        return ResultReponse<IReadOnlyCollection<VehicleImageResponse>>.Success(images);
    }
}
