using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Locations.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Locations.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Domain.Entities.Locations;

namespace Rentify.Backend.Core.Application.Modules.Locations.Commands.CreateTenantLocation;

public sealed class CreateTenantLocationHandler : IRequestHandler<CreateTenantLocationCommand, ResultReponse<TenantLocationResponse>>
{
    private readonly ILocationRepository _locationRepository;
    private readonly ITenantLocationRepository _tenantLocationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTenantLocationHandler(
        ILocationRepository locationRepository,
        ITenantLocationRepository tenantLocationRepository,
        IUnitOfWork unitOfWork)
    {
        _locationRepository = locationRepository;
        _tenantLocationRepository = tenantLocationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultReponse<TenantLocationResponse>> Handle(CreateTenantLocationCommand request, CancellationToken cancellationToken)
    {
        TenantLocation tenantLocation;

        if (request.IsCustom)
        {
            if (request.LocationId.HasValue)
                throw new ApiException("Las ubicaciones personalizadas no pueden tener una ubicación global asociada.", StatusCodes.Status400BadRequest);

            if (string.IsNullOrWhiteSpace(request.DisplayName))
                throw new ApiException("El nombre de la ubicación es requerido.", StatusCodes.Status400BadRequest);

            if (await _tenantLocationRepository.ExistsByDisplayNameAsync(request.TenantId, request.DisplayName, null, cancellationToken))
                throw new ApiException("Ya existe una ubicación con este nombre para tu empresa.", StatusCodes.Status400BadRequest);

            tenantLocation = TenantLocation.CreateCustom(
                request.TenantId,
                request.DisplayName,
                request.AllowsDelivery,
                request.AllowsPickup,
                request.DeliveryFee,
                request.PickupFee,
                request.CreatedBy);
        }
        else
        {
            if (!request.LocationId.HasValue || request.LocationId.Value == Guid.Empty)
                throw new ApiException("La ubicación global es requerida.", StatusCodes.Status400BadRequest);

            Location location = await _locationRepository.GetActiveByIdAsync(request.LocationId.Value, cancellationToken)
                ?? throw new ApiException("Ubicación no encontrada.", StatusCodes.Status404NotFound);

            if (await _tenantLocationRepository.ExistsByLocationAsync(request.TenantId, location.Id, null, cancellationToken))
                throw new ApiException("Esta ubicación ya está configurada para tu empresa.", StatusCodes.Status400BadRequest);

            string displayName = string.IsNullOrWhiteSpace(request.DisplayName) ? location.Name : request.DisplayName;

            if (await _tenantLocationRepository.ExistsByDisplayNameAsync(request.TenantId, displayName, null, cancellationToken))
                throw new ApiException("Ya existe una ubicación con este nombre para tu empresa.", StatusCodes.Status400BadRequest);

            tenantLocation = TenantLocation.CreateFromGlobalLocation(
                request.TenantId,
                location.Id,
                displayName,
                request.AllowsDelivery,
                request.AllowsPickup,
                request.DeliveryFee,
                request.PickupFee,
                request.CreatedBy);
        }

        await _tenantLocationRepository.AddAsync(tenantLocation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        TenantLocationResponse response = await _tenantLocationRepository.GetDetailsAsync(
                request.TenantId,
                tenantLocation.Id,
                cancellationToken)
            ?? throw new ApiException("Ubicación del tenant no encontrada.", StatusCodes.Status404NotFound);

        return ResultReponse<TenantLocationResponse>.Success(response);
    }
}
