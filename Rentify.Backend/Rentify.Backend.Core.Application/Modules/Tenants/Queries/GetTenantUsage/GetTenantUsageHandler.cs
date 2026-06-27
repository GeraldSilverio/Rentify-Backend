using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Tenants.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Queries.GetTenantUsage;

public sealed class GetTenantUsageHandler
    : IRequestHandler<GetTenantUsageQuery, ResultReponse<TenantUsageResponse>>
{
    private readonly ITenantReadRepository _tenantReadRepository;

    public GetTenantUsageHandler(ITenantReadRepository tenantReadRepository)
    {
        _tenantReadRepository = tenantReadRepository;
    }

    public async Task<ResultReponse<TenantUsageResponse>> Handle(
        GetTenantUsageQuery request,
        CancellationToken cancellationToken)
    {
        TenantUsageResponse usage = await _tenantReadRepository.GetUsageAsync(request.TenantId, cancellationToken);
        return ResultReponse<TenantUsageResponse>.Success(usage);
    }
}
