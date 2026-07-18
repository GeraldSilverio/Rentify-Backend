using MediatR;
using Rentify.Backend.Core.Application.Modules.Locations.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Locations.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Locations.Queries.GetTenantLocations;

public sealed class GetTenantLocationsHandler
    : IRequestHandler<GetTenantLocationsQuery, ResultReponse<PaginatedResponse<TenantLocationListItemResponse>>>
{
    private readonly ITenantLocationRepository _tenantLocationRepository;

    public GetTenantLocationsHandler(ITenantLocationRepository tenantLocationRepository)
    {
        _tenantLocationRepository = tenantLocationRepository;
    }

    public async Task<ResultReponse<PaginatedResponse<TenantLocationListItemResponse>>> Handle(
        GetTenantLocationsQuery request,
        CancellationToken cancellationToken)
    {
        PaginatedResponse<TenantLocationListItemResponse> response = await _tenantLocationRepository.GetPagedAsync(request, cancellationToken);
        return ResultReponse<PaginatedResponse<TenantLocationListItemResponse>>.Success(response);
    }
}
