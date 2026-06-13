using MediatR;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Services;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UploadVehicleImage;

public sealed class UploadVehicleImageHandler : IRequestHandler<UploadVehicleImageCommand, ResultReponse<Guid>>
{
    private readonly IVehicleService _vehicleService;

    public UploadVehicleImageHandler(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    public async Task<ResultReponse<Guid>> Handle(UploadVehicleImageCommand request, CancellationToken cancellationToken)
    {
        Guid imageId = await _vehicleService.UploadImageAsync(request, cancellationToken);

        return ResultReponse<Guid>.Success(imageId);
    }
}
