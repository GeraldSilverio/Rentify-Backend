using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Locations.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Locations.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Locations.Queries.GetAdminLocationById;

public sealed class GetAdminLocationByIdHandler : IRequestHandler<GetAdminLocationByIdQuery, ResultReponse<LocationResponse>>
{
    private readonly ILocationRepository _locationRepository;

    public GetAdminLocationByIdHandler(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }

    public async Task<ResultReponse<LocationResponse>> Handle(GetAdminLocationByIdQuery request, CancellationToken cancellationToken)
    {
        LocationResponse response = await _locationRepository.GetDetailsAsync(request.LocationId, cancellationToken)
            ?? throw new ApiException("Ubicación no encontrada.", StatusCodes.Status404NotFound);

        return ResultReponse<LocationResponse>.Success(response);
    }
}
