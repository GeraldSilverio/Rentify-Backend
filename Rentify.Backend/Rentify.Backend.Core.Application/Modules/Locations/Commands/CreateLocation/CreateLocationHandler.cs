using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Locations.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Locations.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Domain.Entities.Locations;

namespace Rentify.Backend.Core.Application.Modules.Locations.Commands.CreateLocation;

public sealed class CreateLocationHandler : IRequestHandler<CreateLocationCommand, ResultReponse<LocationResponse>>
{
    private readonly ILocationRepository _locationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateLocationHandler(ILocationRepository locationRepository, IUnitOfWork unitOfWork)
    {
        _locationRepository = locationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultReponse<LocationResponse>> Handle(CreateLocationCommand request, CancellationToken cancellationToken)
    {
        if (await _locationRepository.ExistsDuplicateAsync(request.Name, request.City, request.Province, request.Country, null, cancellationToken))
            throw new ApiException("Ya existe una ubicación con estos datos.", StatusCodes.Status400BadRequest);

        Location location = Location.Create(
            request.Name,
            request.Type,
            request.City,
            request.Province,
            request.Country,
            request.Address,
            request.Notes,
            request.CreatedBy);

        await _locationRepository.AddAsync(location, cancellationToken);
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
