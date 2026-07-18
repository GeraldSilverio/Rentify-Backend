using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Locations.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Locations.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Locations.Queries.GetTenantLocationById;

public sealed class GetTenantLocationByIdHandler : IRequestHandler<GetTenantLocationByIdQuery, ResultReponse<TenantLocationResponse>>
{
    private readonly ITenantLocationRepository _tenantLocationRepository;

    public GetTenantLocationByIdHandler(ITenantLocationRepository tenantLocationRepository)
    {
        _tenantLocationRepository = tenantLocationRepository;
    }

    public async Task<ResultReponse<TenantLocationResponse>> Handle(GetTenantLocationByIdQuery request, CancellationToken cancellationToken)
    {
        TenantLocationResponse response = await _tenantLocationRepository.GetDetailsAsync(
                request.TenantId,
                request.TenantLocationId,
                cancellationToken)
            ?? throw new ApiException("Ubicación del tenant no encontrada.", StatusCodes.Status404NotFound);

        return ResultReponse<TenantLocationResponse>.Success(response);
    }
}
