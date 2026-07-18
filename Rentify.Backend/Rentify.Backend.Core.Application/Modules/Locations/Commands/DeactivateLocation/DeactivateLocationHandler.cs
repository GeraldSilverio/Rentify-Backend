using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Locations.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Domain.Entities.Locations;

namespace Rentify.Backend.Core.Application.Modules.Locations.Commands.DeactivateLocation;

public sealed class DeactivateLocationHandler : IRequestHandler<DeactivateLocationCommand, ResultReponse<bool>>
{
    private readonly ILocationRepository _locationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateLocationHandler(ILocationRepository locationRepository, IUnitOfWork unitOfWork)
    {
        _locationRepository = locationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultReponse<bool>> Handle(DeactivateLocationCommand request, CancellationToken cancellationToken)
    {
        Location location = await _locationRepository.GetByIdAsync(request.LocationId, cancellationToken)
            ?? throw new ApiException("Ubicación no encontrada.", StatusCodes.Status404NotFound);

        location.Deactivate(request.ModifiedBy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ResultReponse<bool>.Success(true);
    }
}
