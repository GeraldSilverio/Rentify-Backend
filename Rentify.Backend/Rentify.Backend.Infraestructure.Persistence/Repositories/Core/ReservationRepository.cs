using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Core.Application.Modules.Reservations.Contracts.Repositories;
using Rentify.Backend.Core.Domain.Entities;
using Rentify.Backend.Core.Domain.Entities.Reservations;
using Rentify.Backend.Core.Domain.Enums;
using Rentify.Backend.Infraestructure.Persistence.Context;

namespace Rentify.Backend.Infraestructure.Persistence.Repositories;

public sealed class ReservationRepository : IReservationRepository
{
    private readonly RentifyContext _context;

    public ReservationRepository(RentifyContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        await _context.Reservations.AddAsync(reservation, cancellationToken);
    }

    public async Task<Reservation?> GetByIdAsync(Guid tenantId, Guid reservationId, CancellationToken cancellationToken = default)
    {
        return await _context.Reservations
            .Include(x => x.Payments)
            .Include(x => x.Vehicles)
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == reservationId && !x.IsDeleted, cancellationToken);
    }

    public async Task<bool> HasOverlappingReservationAsync(
        Guid tenantId,
        IEnumerable<Guid> vehicleIds,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        Guid[] ids = vehicleIds.ToArray();

        return await _context.ReservationVehicles
            .AnyAsync(x =>
                x.TenantId == tenantId
                && ids.Contains(x.VehicleId)
                && !x.IsDeleted
                && !x.Reservation.IsDeleted
                && x.Reservation.Status != ReservationStatus.Cancelled
                && x.Reservation.StartDate <= endDate
                && x.Reservation.EndDate >= startDate,
                cancellationToken);
    }

    public async Task<decimal> GetMonthlyRevenueAsync(Guid tenantId, int year, int month, CancellationToken cancellationToken = default)
    {
        DateTime from = new(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime to = from.AddMonths(1);

        return await _context.Payments
            .Where(x => x.TenantId == tenantId && !x.IsDeleted && x.PaidAt >= from && x.PaidAt < to)
            .SumAsync(x => x.Amount, cancellationToken);
    }

    public async Task<int> CountActiveReservationsAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Reservations.CountAsync(
            x => x.TenantId == tenantId
                 && !x.IsDeleted
                 && (x.Status == ReservationStatus.Confirmed || x.Status == ReservationStatus.Active),
            cancellationToken);
    }

    public async Task<int> CountUpcomingReturnsAsync(Guid tenantId, DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken = default)
    {
        return await _context.Reservations.CountAsync(
            x => x.TenantId == tenantId
                 && !x.IsDeleted
                 && x.Status != ReservationStatus.Cancelled
                 && x.EndDate >= fromDate
                 && x.EndDate <= toDate,
            cancellationToken);
    }
}
