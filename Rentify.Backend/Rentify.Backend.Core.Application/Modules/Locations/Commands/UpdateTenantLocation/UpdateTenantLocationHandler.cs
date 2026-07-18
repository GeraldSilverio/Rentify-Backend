using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Locations.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Locations.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Domain.Entities.Locations;

namespace Rentify.Backend.Core.Application.Modules.Locations.Commands.UpdateTenantLocation;

public sealed class UpdateTenantLocationHandler : IRequestHandler<UpdateTenantLocationCommand, ResultReponse<TenantLocationResponse>>
{
    private readonly ITenantLocationRepository _tenantLocationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTenantLocationHandler(ITenantLocationRepository tenantLocationRepository, IUnitOfWork unitOfWork)
    {
        _tenantLocationRepository = tenantLocationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultReponse<TenantLocationResponse>> Handle(UpdateTenantLocationCommand request, CancellationToken cancellationToken)
    {
        TenantLocation tenantLocation = await _tenantLocationRepository.GetByIdAsync(
                request.TenantId,
                request.TenantLocationId,
                cancellationToken)
            ?? throw new ApiException("Ubicación del tenant no encontrada.", StatusCodes.Status404NotFound);

        if (await _tenantLocationRepository.ExistsByDisplayNameAsync(request.TenantId, request.DisplayName, request.TenantLocationId, cancellationToken))
            throw new ApiException("Ya existe una ubicación con este nombre para tu empresa.", StatusCodes.Status400BadRequest);

        tenantLocation.Update(
            request.DisplayName,
            request.AllowsDelivery,
            request.AllowsPickup,
            request.DeliveryFee,
            request.PickupFee,
            request.ModifiedBy);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        TenantLocationResponse response = await _tenantLocationRepository.GetDetailsAsync(
                request.TenantId,
                tenantLocation.Id,
                cancellationToken)
            ?? throw new ApiException("Ubicación del tenant no encontrada.", StatusCodes.Status404NotFound);

        return ResultReponse<TenantLocationResponse>.Success(response);
    }
}
