using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Subscriptions.Dtos
{
    public record SubscriptionResponse(
        Guid Id,
        Guid TenantId,
        Guid SubscriptionPlanId,
        string SubscriptionPlanName,
        SubscriptionStatus Status,
        DateTime StartsAt,
        DateTime ExpiresAt,
        DateTime? TrialEndsAt,
        bool IsTrial,
        bool AutoRenew);
}
