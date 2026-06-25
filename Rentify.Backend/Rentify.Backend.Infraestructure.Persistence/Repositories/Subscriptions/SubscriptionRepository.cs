using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Core.Application.Modules.Subscriptions.Contracts.Repositories;
using Rentify.Backend.Core.Application.Shared.Exceptions;
using Rentify.Backend.Core.Domain.Entities.Core;
using Rentify.Backend.Core.Domain.Enums;
using Rentify.Backend.Infraestructure.Persistence.Context;

namespace Rentify.Backend.Infrastructure.Persistence.Repositories;

public sealed class SubscriptionRepository
    : ISubscriptionRepository
{
    private readonly RentifyContext _context;

    public SubscriptionRepository(RentifyContext context)
    {
        _context = context;
    }

    public async Task<SubscriptionPlan?> GetPlanByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return await _context.SubscriptionPlans
            .FirstOrDefaultAsync(
                x => x.Code == code &&
                     x.IsActive,
                cancellationToken);
    }

    public async Task AddAsync(
        Subscription subscription,
        CancellationToken cancellationToken = default)
    {
        await _context.Subscriptions.AddAsync(
            subscription,
            cancellationToken);
    }

    public async Task<Subscription?> GetCurrentByTenantIdAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Subscriptions
            .Include(x => x.SubscriptionPlan)
            .Where(x => x.TenantId == tenantId && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Subscription>> GetSubscriptionsToValidateAsync(
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return await _context.Subscriptions
            .Where(x => !x.IsDeleted)
            .Where(x =>
                (x.Status == SubscriptionStatus.Active && x.ExpiresAt < now) ||
                (x.Status == SubscriptionStatus.Trialing && x.TrialEndsAt != null && x.TrialEndsAt < now) ||
                (x.Status == SubscriptionStatus.Trialing && x.ExpiresAt < now))
            .ToListAsync(cancellationToken);
    }

    public async Task<Guid> GetSubscriptionPlanIdByCodeAsync(string code, CancellationToken cancellationToken = default)
    {

        Guid planId = await _context.SubscriptionPlans
            .Where(x => x.Code == code && x.IsActive)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (planId == Guid.Empty) throw new ApiException("Subscription plan not found.", StatusCodes.Status400BadRequest);

        return planId;
    }
}
