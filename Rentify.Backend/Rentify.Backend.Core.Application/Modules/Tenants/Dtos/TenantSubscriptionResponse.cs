using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Dtos;

public sealed record TenantSubscriptionResponse(
    Guid SubscriptionId,
    Guid PlanId,
    string PlanCode,
    string PlanName,
    PlanType PlanType,
    BillingCycle BillingCycle,
    decimal Price,
    SubscriptionStatus Status,
    DateTime StartsAt,
    DateTime ExpiresAt,
    bool IsTrial,
    DateTime? TrialEndsAt,
    bool AutoRenew,
    TenantSubscriptionLimitsResponse Limits,
    TenantSubscriptionFeaturesResponse Features);
