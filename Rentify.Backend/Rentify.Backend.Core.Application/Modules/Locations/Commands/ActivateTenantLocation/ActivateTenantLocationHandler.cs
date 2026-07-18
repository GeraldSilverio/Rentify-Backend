using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Locations.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Domain.Entities.Locations;

namespace Rentify.Backend.Core.Application.Modules.Locations.Commands.ActivateTenantLocation;

public sealed class ActivateTenantLocationHandler : IRequestHandler<ActivateTenantLocationCommand, ResultReponse<bool>>
{
    private readonly ITenantLocationRepository _tenantLocationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateTenantLocationHandler(ITenantLocationRepository tenantLocationRepository, IUnitOfWork unitOfWork)
    {
        _tenantLocationRepository = tenantLocationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultReponse<bool>> Handle(ActivateTenantLocationCommand request, CancellationToken cancellationToken)
    {
        TenantLocation tenantLocation = await _tenantLocationRepository.GetByIdAsync(request.TenantId, request.TenantLocationId, cancellationToken)
            ?? throw new ApiException("Ubicación del tenant no encontrada.", StatusCodes.Status404NotFound);

        tenantLocation.Activate(request.ModifiedBy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ResultReponse<bool>.Success(true);
    }
}
