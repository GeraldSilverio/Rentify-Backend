using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;
using Rentify.Backend.Core.Domain.Entities.Vehicles;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.CreateVehicleFeature;

public sealed class CreateVehicleFeatureHandler
    : IRequestHandler<CreateVehicleFeatureCommand, ResultReponse<VehicleFeatureResponse>>
{
    private readonly IVehicleCatalogRepository _catalogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateVehicleFeatureHandler(
        IVehicleCatalogRepository catalogRepository,
        IUnitOfWork unitOfWork)
    {
        _catalogRepository = catalogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultReponse<VehicleFeatureResponse>> Handle(
        CreateVehicleFeatureCommand request,
        CancellationToken cancellationToken)
    {
        if (await _catalogRepository.FeatureNameExistsAsync(request.Name, cancellationToken: cancellationToken))
            throw new ApiException("Vehicle feature name already exists.", StatusCodes.Status400BadRequest);

        VehicleFeature feature = VehicleFeature.Create(request.Name, request.Category, request.CreatedBy);
        await _catalogRepository.AddFeatureAsync(feature, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ResultReponse<VehicleFeatureResponse>.Success(
            new VehicleFeatureResponse(feature.Id, feature.Name, feature.Category, feature.IsActive));
    }
}
