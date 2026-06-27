using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Application.Modules.Subscriptions.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Subscriptions.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Tenants.Commands.RegisterTenant;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Domain.Entities.Core;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Subscriptions.Implementations;

public class SubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SubscriptionService(
        ISubscriptionRepository subscriptionRepository,
        IUnitOfWork unitOfWork)
    {
        _subscriptionRepository = subscriptionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Subscription> RegisterSubscriptionAsync(Guid tenantId, RegisterTenantCommand command, CancellationToken cancellationToken)
    {
        var plan = await _subscriptionRepository.GetPlanByCodeAsync(
            command.SubscriptionPlanCode.Trim().ToUpperInvariant(),
            cancellationToken);

        if (plan == null)
        {
            throw new ApiException("Subscription plan not found.", StatusCodes.Status400BadRequest);
        }

        var subscription = Subscription.Create(
            tenantId,
            plan.Id,
            GetDurationInDays(plan.BillingCycle),
            command.TrialDays,
            command.CreatedBy);

        await _subscriptionRepository.AddAsync(subscription, cancellationToken);

        return subscription;
    }

    public async Task<Subscription?> GetCurrentSubscriptionAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _subscriptionRepository.GetCurrentByTenantIdAsync(tenantId, cancellationToken);
    }

    public async Task<ValidateInactiveSubscriptionsResponse> ValidateInactiveSubscriptionsAsync(CancellationToken cancellationToken = default)
    {
        var subscriptions = await _subscriptionRepository.GetSubscriptionsToValidateAsync(cancellationToken);
        var expired = 0;
        var activated = 0;

        foreach (var subscription in subscriptions)
        {
            var previousStatus = subscription.Status;
            subscription.ValidateStatus("system");

            if (previousStatus != subscription.Status && subscription.Status == SubscriptionStatus.Expired)
            {
                expired++;
            }

            if (previousStatus != subscription.Status && subscription.Status == SubscriptionStatus.Active)
            {
                activated++;
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ValidateInactiveSubscriptionsResponse(subscriptions.Count, expired, activated);
    }

    private static int GetDurationInDays(BillingCycle billingCycle)
    {
        return billingCycle switch
        {
            BillingCycle.Monthly => 30,
            BillingCycle.Quarterly => 90,
            BillingCycle.Yearly => 365,
            _ => 30
        };
    }
}
