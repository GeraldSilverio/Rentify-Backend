namespace Rentify.Backend.Core.Application.Modules.Tenants.Dtos;

public sealed record AdminTenantDetailResponse(
    TenantProfileResponse Tenant,
    TenantUsageResponse Usage);
