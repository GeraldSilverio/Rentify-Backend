using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Tenants.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Queries.GetTenantProfile;

public sealed class GetTenantProfileHandler
    : IRequestHandler<GetTenantProfileQuery, ResultReponse<TenantProfileResponse>>
{
    private readonly ITenantReadRepository _tenantReadRepository;

    public GetTenantProfileHandler(ITenantReadRepository tenantReadRepository)
    {
        _tenantReadRepository = tenantReadRepository;
    }

    public async Task<ResultReponse<TenantProfileResponse>> Handle(
        GetTenantProfileQuery request,
        CancellationToken cancellationToken)
    {
        TenantProfileResponse? profile = await _tenantReadRepository.GetProfileAsync(request.TenantId, cancellationToken);

        if (profile is null)
            throw new ApiException("Tenant not found.", StatusCodes.Status404NotFound);

        return ResultReponse<TenantProfileResponse>.Success(profile);
    }
}
