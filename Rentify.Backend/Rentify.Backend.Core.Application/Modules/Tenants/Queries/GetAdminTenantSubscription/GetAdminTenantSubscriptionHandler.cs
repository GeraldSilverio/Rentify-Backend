using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Tenants.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Queries.GetAdminTenantSubscription;

public sealed class GetAdminTenantSubscriptionHandler
    : IRequestHandler<GetAdminTenantSubscriptionQuery, ResultReponse<TenantSubscriptionResponse>>
{
    private readonly ITenantReadRepository _tenantReadRepository;

    public GetAdminTenantSubscriptionHandler(ITenantReadRepository tenantReadRepository)
    {
        _tenantReadRepository = tenantReadRepository;
    }

    public async Task<ResultReponse<TenantSubscriptionResponse>> Handle(
        GetAdminTenantSubscriptionQuery request,
        CancellationToken cancellationToken)
    {
        TenantSubscriptionResponse subscription = await _tenantReadRepository.GetCurrentSubscriptionAsync(request.TenantId, cancellationToken)
            ?? throw new ApiException("Subscription not found.", StatusCodes.Status404NotFound);

        return ResultReponse<TenantSubscriptionResponse>.Success(subscription);
    }
}
