using Rentify.Backend.Core.Domain.Entities;
using Rentify.Backend.Core.Domain.Entities.Reservations;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Contracts.Repositories;

public interface IReservationRepository
{
    Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default);
    Task<Reservation?> GetByIdAsync(Guid tenantId, Guid reservationId, CancellationToken cancellationToken = default);
    Task<bool> HasOverlappingReservationAsync(Guid tenantId, IEnumerable<Guid> vehicleIds, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);
    Task<decimal> GetMonthlyRevenueAsync(Guid tenantId, int year, int month, CancellationToken cancellationToken = default);
    Task<int> CountActiveReservationsAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<int> CountUpcomingReturnsAsync(Guid tenantId, DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken = default);
}
