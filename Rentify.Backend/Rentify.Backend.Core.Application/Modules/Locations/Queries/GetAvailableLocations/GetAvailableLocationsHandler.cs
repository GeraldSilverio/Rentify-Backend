using MediatR;
using Rentify.Backend.Core.Application.Modules.Locations.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Locations.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Locations.Queries.GetAvailableLocations;

public sealed class GetAvailableLocationsHandler
    : IRequestHandler<GetAvailableLocationsQuery, ResultReponse<IReadOnlyCollection<AvailableLocationResponse>>>
{
    private readonly ILocationRepository _locationRepository;

    public GetAvailableLocationsHandler(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }

    public async Task<ResultReponse<IReadOnlyCollection<AvailableLocationResponse>>> Handle(
        GetAvailableLocationsQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<AvailableLocationResponse> response = await _locationRepository.GetAvailableAsync(request, cancellationToken);
        return ResultReponse<IReadOnlyCollection<AvailableLocationResponse>>.Success(response);
    }
}
