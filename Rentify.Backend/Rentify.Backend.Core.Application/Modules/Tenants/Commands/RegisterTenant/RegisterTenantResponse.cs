namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.RegisterTenant;

public sealed record RegisterTenantResponse(
    Guid TenantId,
    Guid SubscriptionId,
    Guid OwnerUserId,
    DateTime SubscriptionExpiresAt,
    DateTime? TrialEndsAt,
    string Message);
