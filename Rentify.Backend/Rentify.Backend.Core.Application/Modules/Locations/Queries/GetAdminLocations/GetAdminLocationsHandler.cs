using MediatR;
using Rentify.Backend.Core.Application.Modules.Locations.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Locations.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Locations.Queries.GetAdminLocations;

public sealed class GetAdminLocationsHandler
    : IRequestHandler<GetAdminLocationsQuery, ResultReponse<PaginatedResponse<LocationListItemResponse>>>
{
    private readonly ILocationRepository _locationRepository;

    public GetAdminLocationsHandler(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }

    public async Task<ResultReponse<PaginatedResponse<LocationListItemResponse>>> Handle(
        GetAdminLocationsQuery request,
        CancellationToken cancellationToken)
    {
        PaginatedResponse<LocationListItemResponse> response = await _locationRepository.GetPagedAsync(request, cancellationToken);
        return ResultReponse<PaginatedResponse<LocationListItemResponse>>.Success(response);
    }
}
