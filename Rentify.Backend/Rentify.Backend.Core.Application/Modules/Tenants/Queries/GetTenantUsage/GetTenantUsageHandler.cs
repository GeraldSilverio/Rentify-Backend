using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Tenants.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Queries.GetTenantUsage;

public sealed class GetTenantUsageHandler
    : IRequestHandler<GetTenantUsageQuery, ResultReponse<TenantUsageResponse>>
{
    private readonly ITenantUsageService _tenantUsageService;

    public GetTenantUsageHandler(ITenantUsageService tenantUsageService)
    {
        _tenantUsageService = tenantUsageService;
    }

    public async Task<ResultReponse<TenantUsageResponse>> Handle(
        GetTenantUsageQuery request,
        CancellationToken cancellationToken)
    {
        TenantUsageResponse usage = await _tenantUsageService.GetUsageAsync(request.TenantId, cancellationToken);
        return ResultReponse<TenantUsageResponse>.Success(usage);
    }
}
