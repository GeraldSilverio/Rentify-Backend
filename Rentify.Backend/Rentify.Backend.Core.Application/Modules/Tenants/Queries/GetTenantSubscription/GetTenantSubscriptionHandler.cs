using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Tenants.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Queries.GetTenantSubscription;

public sealed class GetTenantSubscriptionHandler
    : IRequestHandler<GetTenantSubscriptionQuery, ResultReponse<TenantSubscriptionResponse>>
{
    private readonly ITenantReadRepository _tenantReadRepository;

    public GetTenantSubscriptionHandler(ITenantReadRepository tenantReadRepository)
    {
        _tenantReadRepository = tenantReadRepository;
    }

    public async Task<ResultReponse<TenantSubscriptionResponse>> Handle(
        GetTenantSubscriptionQuery request,
        CancellationToken cancellationToken)
    {
        TenantSubscriptionResponse? subscription = await _tenantReadRepository
            .GetCurrentSubscriptionAsync(request.TenantId, cancellationToken);

        if (subscription is null)
            throw new ApiException("Subscription not found.", StatusCodes.Status404NotFound);

        return ResultReponse<TenantSubscriptionResponse>.Success(subscription);
    }
}
