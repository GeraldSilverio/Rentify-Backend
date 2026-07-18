using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Locations.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Locations.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Domain.Entities.Locations;

namespace Rentify.Backend.Core.Application.Modules.Locations.Commands.UpdateLocation;

public sealed class UpdateLocationHandler : IRequestHandler<UpdateLocationCommand, ResultReponse<LocationResponse>>
{
    private readonly ILocationRepository _locationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLocationHandler(ILocationRepository locationRepository, IUnitOfWork unitOfWork)
    {
        _locationRepository = locationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultReponse<LocationResponse>> Handle(UpdateLocationCommand request, CancellationToken cancellationToken)
    {
        Location location = await _locationRepository.GetByIdAsync(request.LocationId, cancellationToken)
            ?? throw new ApiException("Ubicación no encontrada.", StatusCodes.Status404NotFound);

        if (await _locationRepository.ExistsDuplicateAsync(request.Name, request.City, request.Province, request.Country, request.LocationId, cancellationToken))
            throw new ApiException("Ya existe una ubicación con estos datos.", StatusCodes.Status400BadRequest);

        location.Update(request.Name, request.Type, request.City, request.Province, request.Country, request.Address, request.Notes, request.ModifiedBy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ResultReponse<LocationResponse>.Success(new LocationResponse(
            location.Id,
            location.Name,
            location.Type,
            location.City,
            location.Province,
            location.Country,
            location.Address,
            location.Notes,
            location.IsActive));
    }
}
