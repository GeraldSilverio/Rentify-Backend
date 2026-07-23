using Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicleUnavailablePeriods;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;

public interface IVehicleUnavailableDateRepository
{
    Task<IReadOnlyCollection<VehicleUnavailablePeriodResponse>> GetActivePeriodsAsync(
        Guid tenantId,
        Guid vehicleId,
        DateOnly? fromDate,
        DateOnly? toDate,
        CancellationToken cancellationToken = default);
}
