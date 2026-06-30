using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Tenants.Dtos;
using Rentify.Backend.Core.Domain.Entities.Core;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Services;

public sealed class TenantUsageService : ITenantUsageService
{
    private readonly ITenantReadRepository _tenantReadRepository;
    private readonly ICurrentSubscriptionService _currentSubscriptionService;

    public TenantUsageService(
        ITenantReadRepository tenantReadRepository,
        ICurrentSubscriptionService currentSubscriptionService)
    {
        _tenantReadRepository = tenantReadRepository;
        _currentSubscriptionService = currentSubscriptionService;
    }

    public async Task<TenantUsageResponse> GetUsageAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        Subscription subscription = await _currentSubscriptionService.GetCurrentSubscriptionAsync(tenantId, cancellationToken);
        TenantUsageResponse usage = await _tenantReadRepository.GetUsageAsync(tenantId, cancellationToken);
        SubscriptionPlan plan = subscription.SubscriptionPlan;

        return usage with
        {
            MaxVehicles = plan.MaxVehicles,
            MaxEmployees = plan.MaxEmployees,
            MaxBranches = plan.MaxBranches,
            MaxReservationsPerMonth = plan.MaxReservationsPerMonth
        };
    }

    public async Task EnsureCanCreateVehicleAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        TenantUsageResponse usage = await GetUsageAsync(tenantId, cancellationToken);

        if (usage.MaxVehicles > 0 && usage.VehiclesUsed >= usage.MaxVehicles)
            throw new ApiException("The current subscription vehicle limit has been reached.", StatusCodes.Status403Forbidden);
    }

    public async Task EnsureCanCreateEmployeeAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        TenantUsageResponse usage = await GetUsageAsync(tenantId, cancellationToken);

        if (usage.MaxEmployees > 0 && usage.UsersUsed >= usage.MaxEmployees)
            throw new ApiException("The current subscription employee limit has been reached.", StatusCodes.Status403Forbidden);
    }

    public async Task EnsureCanCreateBranchAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        TenantUsageResponse usage = await GetUsageAsync(tenantId, cancellationToken);

        if (usage.MaxBranches > 0 && usage.BranchesUsed >= usage.MaxBranches)
            throw new ApiException("The current subscription branch limit has been reached.", StatusCodes.Status403Forbidden);
    }

    public async Task EnsureCanCreateReservationAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        TenantUsageResponse usage = await GetUsageAsync(tenantId, cancellationToken);

        if (usage.MaxReservationsPerMonth > 0 && usage.ReservationsThisMonth >= usage.MaxReservationsPerMonth)
            throw new ApiException("The current subscription reservation limit has been reached.", StatusCodes.Status403Forbidden);
    }

    public async Task EnsureFeatureEnabledAsync(
        Guid tenantId,
        string featureCode,
        CancellationToken cancellationToken = default)
    {
        Subscription subscription = await _currentSubscriptionService.GetCurrentSubscriptionAsync(tenantId, cancellationToken);
        SubscriptionPlan plan = subscription.SubscriptionPlan;

        bool isEnabled = featureCode.Trim().ToUpperInvariant() switch
        {
            "MULTI_BRANCH" => plan.MultiBranchEnabled,
            "REPORTS" => plan.ReportsEnabled,
            "API_ACCESS" => plan.ApiAccessEnabled,
            "CONTRACTS" => plan.ContractsEnabled,
            "MAINTENANCE" => plan.MaintenanceModuleEnabled,
            "PRIORITY_SUPPORT" => plan.PrioritySupportEnabled,
            "WHITE_LABEL" => plan.WhiteLabelEnabled,
            _ => false
        };

        if (!isEnabled)
            throw new ApiException("The current subscription plan does not include this feature.", StatusCodes.Status403Forbidden);
    }
}
