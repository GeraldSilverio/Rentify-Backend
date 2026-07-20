using Rentify.Backend.Core.Application.Modules.Reservations.Dtos;
using Rentify.Backend.Core.Application.Modules.Reservations.Queries;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Domain.Entities.Reservations;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Contracts.Repositories;

public interface IReservationRepository
{
    Task<Reservation?> GetByIdAsync(Guid tenantId, Guid reservationId, CancellationToken cancellationToken = default);
    Task<ReservationDetailsResponse?> GetDetailsAsync(Guid tenantId, Guid reservationId, CancellationToken cancellationToken = default);
    Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(Guid tenantId, string code, CancellationToken cancellationToken = default);
    Task<string?> GetLastCodeAsync(Guid tenantId, int year, CancellationToken cancellationToken = default);
    Task<bool> HasApprovedOverlapAsync(Guid tenantId, Guid vehicleId, DateTime deliveryDateTime, DateTime expectedReturnDateTime, Guid? excludedReservationId, CancellationToken cancellationToken = default);
    Task<PaginatedResponse<ReservationListItemResponse>> GetPagedAsync(GetReservationsQuery query, CancellationToken cancellationToken = default);
}
