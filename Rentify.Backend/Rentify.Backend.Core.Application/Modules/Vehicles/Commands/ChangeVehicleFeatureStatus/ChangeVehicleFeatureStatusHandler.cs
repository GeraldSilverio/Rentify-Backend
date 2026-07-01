using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;
using Rentify.Backend.Core.Domain.Entities.Vehicles;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.ChangeVehicleFeatureStatus;

public sealed class ChangeVehicleFeatureStatusHandler
    : IRequestHandler<ChangeVehicleFeatureStatusCommand, ResultReponse<VehicleFeatureResponse>>
{
    private readonly IVehicleCatalogRepository _catalogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeVehicleFeatureStatusHandler(IVehicleCatalogRepository catalogRepository, IUnitOfWork unitOfWork)
    {
        _catalogRepository = catalogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultReponse<VehicleFeatureResponse>> Handle(
        ChangeVehicleFeatureStatusCommand request,
        CancellationToken cancellationToken)
    {
        VehicleFeature feature = await _catalogRepository.GetFeatureByIdAsync(request.FeatureId, cancellationToken)
                                 ?? throw new ApiException("Vehicle feature not found.", StatusCodes.Status404NotFound);

        if (request.IsActive)
            feature.Activate(request.ModifiedBy);
        else
            feature.Deactivate(request.ModifiedBy);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ResultReponse<VehicleFeatureResponse>.Success(
            new VehicleFeatureResponse(feature.Id, feature.Name, feature.Category, feature.IsActive));
    }
}
