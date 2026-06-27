using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Tenants.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Queries.GetAdminTenantDetail;

public sealed class GetAdminTenantDetailHandler
    : IRequestHandler<GetAdminTenantDetailQuery, ResultReponse<AdminTenantDetailResponse>>
{
    private readonly ITenantReadRepository _tenantReadRepository;

    public GetAdminTenantDetailHandler(ITenantReadRepository tenantReadRepository)
    {
        _tenantReadRepository = tenantReadRepository;
    }

    public async Task<ResultReponse<AdminTenantDetailResponse>> Handle(
        GetAdminTenantDetailQuery request,
        CancellationToken cancellationToken)
    {
        TenantProfileResponse tenant = await _tenantReadRepository.GetProfileAsync(request.TenantId, cancellationToken)
            ?? throw new ApiException("Tenant not found.", StatusCodes.Status404NotFound);

        TenantUsageResponse usage = await _tenantReadRepository.GetUsageAsync(request.TenantId, cancellationToken);

        return ResultReponse<AdminTenantDetailResponse>.Success(new AdminTenantDetailResponse(tenant, usage));
    }
}
