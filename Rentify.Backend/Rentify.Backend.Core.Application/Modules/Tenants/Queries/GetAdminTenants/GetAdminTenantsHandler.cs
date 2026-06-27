using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Tenants.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Queries.GetAdminTenants;

public sealed class GetAdminTenantsHandler
    : IRequestHandler<GetAdminTenantsQuery, ResultReponse<PaginatedResponse<AdminTenantListItemResponse>>>
{
    private readonly ITenantReadRepository _tenantReadRepository;

    public GetAdminTenantsHandler(ITenantReadRepository tenantReadRepository)
    {
        _tenantReadRepository = tenantReadRepository;
    }

    public async Task<ResultReponse<PaginatedResponse<AdminTenantListItemResponse>>> Handle(
        GetAdminTenantsQuery request,
        CancellationToken cancellationToken)
    {
        PaginatedResponse<AdminTenantListItemResponse> response =
            await _tenantReadRepository.GetAdminTenantsAsync(request, cancellationToken);

        return ResultReponse<PaginatedResponse<AdminTenantListItemResponse>>.Success(response);
    }
}
