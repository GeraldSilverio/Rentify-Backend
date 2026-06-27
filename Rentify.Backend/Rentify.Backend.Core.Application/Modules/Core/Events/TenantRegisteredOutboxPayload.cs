using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Events
{
    public sealed record TenantRegisteredOutboxPayload(
    Guid TenantId,
    Guid SubscriptionId,
    Guid OwnerUserId,
    string TenantName,
    string OwnerFullName,
    string OwnerEmail,
    string SubscriptionPlanCode,
    string SubscriptionStatus,
    DateTime SubscriptionStartsAt,
    DateTime SubscriptionExpiresAt,
    DateTime? TrialEndsAt,
    BusinessModel BusinessModel);
}
