using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Dtos;

public sealed record AdminTenantListItemResponse(
    Guid TenantId,
    string Name,
    string? LegalName,
    string? Rnc,
    BusinessModel BusinessModel,
    bool IsActive,
    DateTime CreatedDate,
    string? CurrentPlanName,
    SubscriptionStatus? SubscriptionStatus,
    DateTime? TrialEndsAt,
    DateTime? ExpiresAt);
