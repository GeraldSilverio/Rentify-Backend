using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Core.Application.Modules.Dashboard.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Dashboard.Dtos;
using Rentify.Backend.Core.Domain.Enums;
using Rentify.Backend.Infraestructure.Persistence.Context;

namespace Rentify.Backend.Infraestructure.Persistence.Repositories;

public sealed class DashboardRepository : IDashboardRepository
{
    private readonly RentifyContext _context;

    public DashboardRepository(RentifyContext context)
    {
        _context = context;
    }

    public async Task<DashboardMetricsResponse> GetMetricsAsync(Guid tenantId, DateOnly today, CancellationToken cancellationToken = default)
    {
        DateTime monthStart = new(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime nextMonth = monthStart.AddMonths(1);
        DateOnly upcomingLimit = today.AddDays(7);

        decimal monthlyRevenue = await _context.Payments
            .Where(x => x.TenantId == tenantId && !x.IsDeleted && x.PaidAt >= monthStart && x.PaidAt < nextMonth)
            .SumAsync(x => x.Amount, cancellationToken);

        int activeReservations = await _context.Reservations
            .CountAsync(x => x.TenantId == tenantId
                             && !x.IsDeleted
                             && (x.Status == ReservationStatus.Confirmed || x.Status == ReservationStatus.Active),
                cancellationToken);

        int availableVehicles = await _context.Vehicles
            .CountAsync(x => x.TenantId == tenantId
                             && !x.IsDeleted
                             && x.Status == VehicleStatus.Available,
                cancellationToken);

        int upcomingReturns = await _context.Reservations
            .CountAsync(x => x.TenantId == tenantId
                             && !x.IsDeleted
                             && x.Status != ReservationStatus.Cancelled
                             && x.EndDate >= today
                             && x.EndDate <= upcomingLimit,
                cancellationToken);

        return new DashboardMetricsResponse(
            monthlyRevenue,
            activeReservations,
            availableVehicles,
            upcomingReturns);
    }
}
