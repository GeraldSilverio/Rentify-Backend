using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Subscriptions.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Tenants.Dtos;
using Rentify.Backend.Core.Domain.Entities.Core;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Services;

public sealed class CurrentSubscriptionService : ICurrentSubscriptionService
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public CurrentSubscriptionService(ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<Subscription> GetCurrentSubscriptionAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        Subscription subscription = await _subscriptionRepository.GetCurrentByTenantIdAsync(tenantId, cancellationToken)
            ?? throw new ApiException("Subscription not found.", StatusCodes.Status404NotFound);

        ValidateSubscription(subscription);

        return subscription;
    }

    public async Task<TenantSubscriptionResponse> GetCurrentSubscriptionResponseAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        Subscription subscription = await GetCurrentSubscriptionAsync(tenantId, cancellationToken);

        return ToResponse(subscription);
    }

    private static void ValidateSubscription(Subscription subscription)
    {
        if (subscription.IsDeleted)
            throw new ApiException("Subscription not found.", StatusCodes.Status404NotFound);

        if (subscription.Status is not SubscriptionStatus.Active and not SubscriptionStatus.Trialing)
            throw new ApiException("The current subscription is not active.", StatusCodes.Status403Forbidden);

        if (subscription.ExpiresAt < DateTime.UtcNow)
            throw new ApiException("The current subscription has expired.", StatusCodes.Status403Forbidden);

        if (subscription.SubscriptionPlan is null ||
            subscription.SubscriptionPlan.IsDeleted ||
            !subscription.SubscriptionPlan.IsActive)
        {
            throw new ApiException("The current subscription plan is not active.", StatusCodes.Status403Forbidden);
        }
    }

    private static TenantSubscriptionResponse ToResponse(Subscription subscription)
    {
        SubscriptionPlan plan = subscription.SubscriptionPlan;

        return new TenantSubscriptionResponse(
            subscription.Id,
            plan.Id,
            plan.Code,
            plan.Name,
            plan.Type,
            plan.BillingCycle,
            plan.Price,
            subscription.Status,
            subscription.StartsAt,
            subscription.ExpiresAt,
            subscription.IsTrial,
            subscription.TrialEndsAt,
            subscription.AutoRenew,
            new TenantSubscriptionLimitsResponse(
                plan.MaxVehicles,
                plan.MaxEmployees,
                plan.MaxBranches,
                plan.MaxReservationsPerMonth),
            new TenantSubscriptionFeaturesResponse(
                plan.MultiBranchEnabled,
                plan.ReportsEnabled,
                plan.ApiAccessEnabled,
                plan.ContractsEnabled,
                plan.MaintenanceModuleEnabled,
                plan.PrioritySupportEnabled,
                plan.WhiteLabelEnabled));
    }
}
