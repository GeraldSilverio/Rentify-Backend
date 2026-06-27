using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Dtos;

public sealed record TenantProfileResponse(
    Guid TenantId,
    string Name,
    string? LegalName,
    string? Rnc,
    BusinessModel BusinessModel,
    bool IsActive,
    DateTime CreatedDate,
    TenantSettingsResponse? TenantSettings,
    TenantPaymentPolicyResponse? PaymentPolicy,
    TenantSubscriptionResponse? CurrentSubscription);
