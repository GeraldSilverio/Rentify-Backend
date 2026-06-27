using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Tenants.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Queries.GetTenantSettings;

public sealed class GetTenantSettingsHandler
    : IRequestHandler<GetTenantSettingsQuery, ResultReponse<TenantSettingsResponse>>
{
    private readonly ITenantReadRepository _tenantReadRepository;

    public GetTenantSettingsHandler(ITenantReadRepository tenantReadRepository)
    {
        _tenantReadRepository = tenantReadRepository;
    }

    public async Task<ResultReponse<TenantSettingsResponse>> Handle(
        GetTenantSettingsQuery request,
        CancellationToken cancellationToken)
    {
        TenantSettingsResponse? settings = await _tenantReadRepository.GetSettingsAsync(request.TenantId, cancellationToken);

        if (settings is null)
            throw new ApiException("Tenant settings not found.", StatusCodes.Status404NotFound);

        return ResultReponse<TenantSettingsResponse>.Success(settings);
    }
}
