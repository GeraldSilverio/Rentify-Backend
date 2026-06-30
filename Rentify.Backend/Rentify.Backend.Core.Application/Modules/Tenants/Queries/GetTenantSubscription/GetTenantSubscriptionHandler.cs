using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Tenants.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Queries.GetTenantSubscription;

public sealed class GetTenantSubscriptionHandler
    : IRequestHandler<GetTenantSubscriptionQuery, ResultReponse<TenantSubscriptionResponse>>
{
    private readonly ICurrentSubscriptionService _currentSubscriptionService;

    public GetTenantSubscriptionHandler(ICurrentSubscriptionService currentSubscriptionService)
    {
        _currentSubscriptionService = currentSubscriptionService;
    }

    public async Task<ResultReponse<TenantSubscriptionResponse>> Handle(
        GetTenantSubscriptionQuery request,
        CancellationToken cancellationToken)
    {
        TenantSubscriptionResponse subscription = await _currentSubscriptionService
            .GetCurrentSubscriptionResponseAsync(request.TenantId, cancellationToken);

        return ResultReponse<TenantSubscriptionResponse>.Success(subscription);
    }
}
