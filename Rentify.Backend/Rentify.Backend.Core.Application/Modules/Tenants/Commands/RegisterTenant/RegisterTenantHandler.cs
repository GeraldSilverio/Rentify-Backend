using MediatR;
using Rentify.Backend.Core.Application.Modules.Subscriptions.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Services;
using Rentify.Backend.Core.Application.Shared.Response;
using Rentify.Backend.Core.Application.Shared.UnitOfWork;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.RegisterTenant;

public sealed class RegisterTenantHandler : IRequestHandler<RegisterTenantCommand, ResultReponse<RegisterTenantResponse>>
{
    private readonly ITenantService _tenantService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterTenantHandler(
        ITenantService tenantService,
        ISubscriptionService subscriptionService,
        IUnitOfWork unitOfWork)
    {
        _tenantService = tenantService;
        _subscriptionService = subscriptionService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultReponse<RegisterTenantResponse>> Handle(
        RegisterTenantCommand request,
        CancellationToken cancellationToken)
    {
        var tenantId = await _tenantService.CreateTenantAsync(request, cancellationToken);
        var subscription = await _subscriptionService.RegisterSubscriptionAsync(tenantId, request, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ResultReponse<RegisterTenantResponse>.Success(new RegisterTenantResponse(
            tenantId,
            subscription.Id,
            subscription.ExpiresAt,
            subscription.TrialEndsAt,
            "Successfully registered tenant."));
    }
}
