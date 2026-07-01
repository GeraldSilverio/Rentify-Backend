using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;
using Rentify.Backend.Core.Domain.Entities.Vehicles;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UpdateVehicleFeature;

public sealed class UpdateVehicleFeatureHandler
    : IRequestHandler<UpdateVehicleFeatureCommand, ResultReponse<VehicleFeatureResponse>>
{
    private readonly IVehicleCatalogRepository _catalogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateVehicleFeatureHandler(IVehicleCatalogRepository catalogRepository, IUnitOfWork unitOfWork)
    {
        _catalogRepository = catalogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultReponse<VehicleFeatureResponse>> Handle(
        UpdateVehicleFeatureCommand request,
        CancellationToken cancellationToken)
    {
        VehicleFeature feature = await _catalogRepository.GetFeatureByIdAsync(request.FeatureId, cancellationToken)
                                 ?? throw new ApiException("Vehicle feature not found.", StatusCodes.Status404NotFound);

        if (await _catalogRepository.FeatureNameExistsAsync(request.Name, request.FeatureId, cancellationToken))
            throw new ApiException("Vehicle feature name already exists.", StatusCodes.Status400BadRequest);

        feature.Update(request.Name, request.Category, request.ModifiedBy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ResultReponse<VehicleFeatureResponse>.Success(
            new VehicleFeatureResponse(feature.Id, feature.Name, feature.Category, feature.IsActive));
    }
}
