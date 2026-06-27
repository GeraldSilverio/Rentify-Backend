using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Tenants.Dtos;
using Rentify.Backend.Core.Application.Modules.Tenants.Queries.GetAdminTenants;
using Rentify.Backend.Core.Domain.Enums;
using Rentify.Backend.Infraestructure.Persistence.Context;

namespace Rentify.Backend.Infraestructure.Persistence.Repositories.Core;

public sealed class TenantReadRepository : ITenantReadRepository
{
    private readonly RentifyContext _context;

    public TenantReadRepository(RentifyContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResponse<AdminTenantListItemResponse>> GetAdminTenantsAsync(
        GetAdminTenantsQuery query,
        CancellationToken cancellationToken = default)
    {
        var tenantsQuery = _context.Tenants
            .AsNoTracking()
            .Where(tenant => !tenant.IsDeleted);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string search = $"%{query.Search.Trim()}%";

            tenantsQuery = tenantsQuery.Where(tenant =>
                EF.Functions.ILike(tenant.Name, search) ||
                (tenant.LegalName != null && EF.Functions.ILike(tenant.LegalName, search)) ||
                (tenant.Rnc != null && EF.Functions.ILike(tenant.Rnc, search)));
        }

        if (query.BusinessModel.HasValue)
            tenantsQuery = tenantsQuery.Where(tenant => tenant.BusinessModel == query.BusinessModel.Value);

        if (query.IsActive.HasValue)
            tenantsQuery = tenantsQuery.Where(tenant => tenant.IsActive == query.IsActive.Value);

        var projectedQuery = tenantsQuery.Select(tenant => new
        {
            Tenant = tenant,
            CurrentSubscription = _context.Subscriptions
                .Where(subscription => subscription.TenantId == tenant.Id && !subscription.IsDeleted)
                .OrderByDescending(subscription => subscription.StartsAt)
                .Select(subscription => new
                {
                    subscription.Status,
                    subscription.TrialEndsAt,
                    subscription.ExpiresAt,
                    PlanCode = subscription.SubscriptionPlan.Code,
                    PlanName = subscription.SubscriptionPlan.Name
                })
                .FirstOrDefault()
        });

        if (query.SubscriptionStatus.HasValue)
        {
            projectedQuery = projectedQuery.Where(x =>
                x.CurrentSubscription != null &&
                x.CurrentSubscription.Status == query.SubscriptionStatus.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.PlanCode))
        {
            string planCode = query.PlanCode.Trim().ToUpperInvariant();

            projectedQuery = projectedQuery.Where(x =>
                x.CurrentSubscription != null &&
                x.CurrentSubscription.PlanCode.ToUpper() == planCode);
        }

        int totalCount = await projectedQuery.CountAsync(cancellationToken);
        int totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

        IReadOnlyList<AdminTenantListItemResponse> items = await projectedQuery
            .OrderByDescending(x => x.Tenant.CreatedDate)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => new AdminTenantListItemResponse(
                x.Tenant.Id,
                x.Tenant.Name,
                x.Tenant.LegalName,
                x.Tenant.Rnc,
                x.Tenant.BusinessModel,
                x.Tenant.IsActive,
                x.Tenant.CreatedDate,
                x.CurrentSubscription == null ? null : x.CurrentSubscription.PlanName,
                x.CurrentSubscription == null ? null : x.CurrentSubscription.Status,
                x.CurrentSubscription == null ? null : x.CurrentSubscription.TrialEndsAt,
                x.CurrentSubscription == null ? null : x.CurrentSubscription.ExpiresAt))
            .ToListAsync(cancellationToken);

        return new PaginatedResponse<AdminTenantListItemResponse>(
            items,
            query.PageNumber,
            query.PageSize,
            totalCount,
            totalPages);
    }

    public async Task<TenantProfileResponse?> GetProfileAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var tenant = await _context.Tenants
            .AsNoTracking()
            .Where(x => x.Id == tenantId && !x.IsDeleted)
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.LegalName,
                x.Rnc,
                x.BusinessModel,
                x.IsActive,
                x.CreatedDate
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (tenant is null)
            return null;

        TenantSettingsResponse? settings = await GetSettingsAsync(tenantId, cancellationToken);
        TenantPaymentPolicyResponse? paymentPolicy = await GetDefaultPaymentPolicyAsync(tenantId, cancellationToken);
        TenantSubscriptionResponse? subscription = await GetCurrentSubscriptionAsync(tenantId, cancellationToken);

        return new TenantProfileResponse(
            tenant.Id,
            tenant.Name,
            tenant.LegalName,
            tenant.Rnc,
            tenant.BusinessModel,
            tenant.IsActive,
            tenant.CreatedDate,
            settings,
            paymentPolicy,
            subscription);
    }

    public async Task<TenantSettingsResponse?> GetSettingsAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.TenantSettings
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && !x.IsDeleted)
            .Select(x => new TenantSettingsResponse(
                x.CurrencyCode,
                x.TimeZone,
                x.EnableReservations,
                x.EnableDriverFleet,
                x.EnableMaintenance,
                x.EnableLateFees,
                x.EnablePublicCatalog))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TenantPaymentPolicyResponse?> GetDefaultPaymentPolicyAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.PaymentPolicies
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.IsDefault && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedDate)
            .Select(x => new TenantPaymentPolicyResponse(
                x.Id,
                x.Name,
                x.PaymentFrequency,
                x.CutoffDayOfWeek,
                x.GraceDays,
                x.ReminderStartDayOfWeek,
                x.LateFeeEnabled,
                x.IsDefault))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TenantSubscriptionResponse?> GetCurrentSubscriptionAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Subscriptions
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId
                        && !x.IsDeleted
                        && (x.Status == SubscriptionStatus.Active || x.Status == SubscriptionStatus.Trialing))
            .OrderByDescending(x => x.StartsAt)
            .Select(x => new TenantSubscriptionResponse(
                x.Id,
                x.SubscriptionPlanId,
                x.SubscriptionPlan.Code,
                x.SubscriptionPlan.Name,
                x.SubscriptionPlan.Type,
                x.SubscriptionPlan.BillingCycle,
                x.SubscriptionPlan.Price,
                x.Status,
                x.StartsAt,
                x.ExpiresAt,
                x.IsTrial,
                x.TrialEndsAt,
                x.AutoRenew,
                new TenantSubscriptionLimitsResponse(
                    x.SubscriptionPlan.MaxVehicles,
                    x.SubscriptionPlan.MaxEmployees,
                    x.SubscriptionPlan.MaxBranches,
                    x.SubscriptionPlan.MaxReservationsPerMonth),
                new TenantSubscriptionFeaturesResponse(
                    x.SubscriptionPlan.MultiBranchEnabled,
                    x.SubscriptionPlan.ReportsEnabled,
                    x.SubscriptionPlan.ApiAccessEnabled,
                    x.SubscriptionPlan.ContractsEnabled,
                    x.SubscriptionPlan.MaintenanceModuleEnabled,
                    x.SubscriptionPlan.PrioritySupportEnabled,
                    x.SubscriptionPlan.WhiteLabelEnabled)))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TenantUsageResponse> GetUsageAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        TenantSubscriptionLimitsResponse limits = await _context.Subscriptions
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId
                        && !x.IsDeleted
                        && (x.Status == SubscriptionStatus.Active || x.Status == SubscriptionStatus.Trialing))
            .OrderByDescending(x => x.StartsAt)
            .Select(x => new TenantSubscriptionLimitsResponse(
                x.SubscriptionPlan.MaxVehicles,
                x.SubscriptionPlan.MaxEmployees,
                x.SubscriptionPlan.MaxBranches,
                x.SubscriptionPlan.MaxReservationsPerMonth))
            .FirstOrDefaultAsync(cancellationToken)
            ?? new TenantSubscriptionLimitsResponse(0, 0, 0, 0);

        int vehiclesUsed = await _context.Vehicles
            .AsNoTracking()
            .CountAsync(x => x.TenantId == tenantId && !x.IsDeleted, cancellationToken);

        DateTime utcNow = DateTime.UtcNow;
        DateOnly monthStart = new(utcNow.Year, utcNow.Month, 1);
        DateOnly nextMonthStart = monthStart.AddMonths(1);

        int reservationsThisMonth = await _context.Reservations
            .AsNoTracking()
            .CountAsync(
                x => x.TenantId == tenantId
                     && !x.IsDeleted
                     && x.CreatedDate >= monthStart.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc)
                     && x.CreatedDate < nextMonthStart.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
                cancellationToken);

        return new TenantUsageResponse(
            vehiclesUsed,
            limits.MaxVehicles,
            0,
            limits.MaxEmployees,
            0,
            limits.MaxBranches,
            reservationsThisMonth,
            limits.MaxReservationsPerMonth);
    }
}
