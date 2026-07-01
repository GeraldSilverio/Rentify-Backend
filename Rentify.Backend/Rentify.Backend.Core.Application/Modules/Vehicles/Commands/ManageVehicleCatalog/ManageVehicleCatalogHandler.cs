using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;
using Rentify.Backend.Core.Domain.Entities.Vehicles;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.ManageVehicleCatalog;

public sealed class ManageVehicleCatalogHandler
    : IRequestHandler<ManageVehicleCatalogCommand, ResultReponse<object>>
{
    private readonly IVehicleCatalogRepository _catalogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ManageVehicleCatalogHandler(
        IVehicleCatalogRepository catalogRepository,
        IUnitOfWork unitOfWork)
    {
        _catalogRepository = catalogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultReponse<object>> Handle(
        ManageVehicleCatalogCommand request,
        CancellationToken cancellationToken)
    {
        object response = request.CatalogKind switch
        {
            VehicleCatalogKind.Brand => await HandleBrandAsync(request, cancellationToken),
            VehicleCatalogKind.Model => await HandleModelAsync(request, cancellationToken),
            VehicleCatalogKind.Type => await HandleTypeAsync(request, cancellationToken),
            _ => throw new ApiException("Invalid catalog type.", StatusCodes.Status400BadRequest)
        };

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ResultReponse<object>.Success(response);
    }

    private async Task<object> HandleBrandAsync(ManageVehicleCatalogCommand request, CancellationToken cancellationToken)
    {
        if (request.Action == VehicleCatalogAction.Create)
        {
            await EnsureBrandNameIsUniqueAsync(request.Name!, null, cancellationToken);
            VehicleBrand brand = VehicleBrand.Create(request.Name!, request.ModifiedBy);
            await _catalogRepository.AddBrandAsync(brand, cancellationToken);
            return new VehicleBrandResponse(brand.Id, brand.Name);
        }

        VehicleBrand existing = await _catalogRepository.GetBrandByIdAsync(request.CatalogId!.Value, cancellationToken)
                                ?? throw new ApiException("Vehicle brand not found.", StatusCodes.Status404NotFound);

        if (request.Action == VehicleCatalogAction.Update)
        {
            await EnsureBrandNameIsUniqueAsync(request.Name!, existing.Id, cancellationToken);
            existing.Update(request.Name!, request.ModifiedBy);
        }
        else if (request.Action == VehicleCatalogAction.Activate)
        {
            existing.Activate(request.ModifiedBy);
        }
        else
        {
            existing.Deactivate(request.ModifiedBy);
        }

        return new VehicleBrandResponse(existing.Id, existing.Name);
    }

    private async Task<object> HandleModelAsync(ManageVehicleCatalogCommand request, CancellationToken cancellationToken)
    {
        if (request.Action is VehicleCatalogAction.Create or VehicleCatalogAction.Update)
        {
            if (!await _catalogRepository.VehicleBrandExistsAsync(request.VehicleBrandId!.Value, cancellationToken))
                throw new ApiException("Vehicle brand not found or inactive.", StatusCodes.Status400BadRequest);
        }

        if (request.Action == VehicleCatalogAction.Create)
        {
            await EnsureModelNameIsUniqueAsync(request.VehicleBrandId!.Value, request.Name!, null, cancellationToken);
            VehicleModel model = VehicleModel.Create(request.VehicleBrandId.Value, request.Name!, request.ModifiedBy);
            await _catalogRepository.AddModelAsync(model, cancellationToken);
            VehicleBrand brand = await _catalogRepository.GetBrandByIdAsync(model.VehicleBrandId, cancellationToken)
                                 ?? throw new ApiException("Vehicle brand not found.", StatusCodes.Status404NotFound);
            return new VehicleModelResponse(model.Id, model.Name, model.VehicleBrandId, brand.Name);
        }

        VehicleModel existing = await _catalogRepository.GetModelByIdAsync(request.CatalogId!.Value, cancellationToken)
                                ?? throw new ApiException("Vehicle model not found.", StatusCodes.Status404NotFound);

        if (request.Action == VehicleCatalogAction.Update)
        {
            await EnsureModelNameIsUniqueAsync(request.VehicleBrandId!.Value, request.Name!, existing.Id, cancellationToken);
            existing.Update(request.VehicleBrandId.Value, request.Name!, request.ModifiedBy);
        }
        else if (request.Action == VehicleCatalogAction.Activate)
        {
            existing.Activate(request.ModifiedBy);
        }
        else
        {
            existing.Deactivate(request.ModifiedBy);
        }

        VehicleBrand existingBrand = await _catalogRepository.GetBrandByIdAsync(existing.VehicleBrandId, cancellationToken)
                                     ?? throw new ApiException("Vehicle brand not found.", StatusCodes.Status404NotFound);
        return new VehicleModelResponse(existing.Id, existing.Name, existing.VehicleBrandId, existingBrand.Name);
    }

    private async Task<object> HandleTypeAsync(ManageVehicleCatalogCommand request, CancellationToken cancellationToken)
    {
        if (request.Action == VehicleCatalogAction.Create)
        {
            await EnsureTypeNameIsUniqueAsync(request.Name!, null, cancellationToken);
            VehicleType type = VehicleType.Create(request.Name!, request.ModifiedBy);
            await _catalogRepository.AddTypeAsync(type, cancellationToken);
            return new VehicleTypeResponse(type.Id, type.Name, type.IsActive);
        }

        VehicleType existing = await _catalogRepository.GetTypeByIdAsync(request.CatalogId!.Value, cancellationToken)
                               ?? throw new ApiException("Vehicle type not found.", StatusCodes.Status404NotFound);

        if (request.Action == VehicleCatalogAction.Update)
        {
            await EnsureTypeNameIsUniqueAsync(request.Name!, existing.Id, cancellationToken);
            existing.Update(request.Name!, request.ModifiedBy);
        }
        else if (request.Action == VehicleCatalogAction.Activate)
        {
            existing.Activate(request.ModifiedBy);
        }
        else
        {
            existing.Deactivate(request.ModifiedBy);
        }

        return new VehicleTypeResponse(existing.Id, existing.Name, existing.IsActive);
    }

    private async Task EnsureBrandNameIsUniqueAsync(string name, Guid? excludedId, CancellationToken cancellationToken)
    {
        if (await _catalogRepository.BrandNameExistsAsync(name, excludedId, cancellationToken))
            throw new ApiException("Vehicle brand name already exists.", StatusCodes.Status400BadRequest);
    }

    private async Task EnsureModelNameIsUniqueAsync(Guid brandId, string name, Guid? excludedId, CancellationToken cancellationToken)
    {
        if (await _catalogRepository.ModelNameExistsAsync(brandId, name, excludedId, cancellationToken))
            throw new ApiException("Vehicle model name already exists for this brand.", StatusCodes.Status400BadRequest);
    }

    private async Task EnsureTypeNameIsUniqueAsync(string name, Guid? excludedId, CancellationToken cancellationToken)
    {
        if (await _catalogRepository.TypeNameExistsAsync(name, excludedId, cancellationToken))
            throw new ApiException("Vehicle type name already exists.", StatusCodes.Status400BadRequest);
    }
}
