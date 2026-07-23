using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicleUnavailablePeriods;
using Rentify.Backend.Core.Domain.Entities.Vehicles;
using Rentify.Backend.Infraestructure.Persistence.Context;

namespace Rentify.Backend.Infraestructure.Persistence.Repositories;

public sealed class VehicleUnavailableDateRepository : IVehicleUnavailableDateRepository
{
    private readonly RentifyContext _context;

    public VehicleUnavailableDateRepository(RentifyContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<VehicleUnavailablePeriodResponse>> GetActivePeriodsAsync(
        Guid tenantId,
        Guid vehicleId,
        DateOnly? fromDate,
        DateOnly? toDate,
        CancellationToken cancellationToken = default)
    {
        IQueryable<VehicleUnavailableDate> unavailableDates = _context.VehicleUnavailableDates
            .AsNoTracking()
            .Where(unavailableDate =>
                unavailableDate.TenantId == tenantId
                && unavailableDate.VehicleId == vehicleId
                && unavailableDate.IsActive
                && !unavailableDate.IsDeleted
                && unavailableDate.StartDate <= unavailableDate.EndDate);

        if (fromDate.HasValue)
            unavailableDates = unavailableDates.Where(unavailableDate => unavailableDate.EndDate >= fromDate.Value);

        if (toDate.HasValue)
            unavailableDates = unavailableDates.Where(unavailableDate => unavailableDate.StartDate <= toDate.Value);

        return await unavailableDates
            .OrderBy(unavailableDate => unavailableDate.StartDate)
            .Select(unavailableDate => new VehicleUnavailablePeriodResponse(
                unavailableDate.StartDate,
                unavailableDate.EndDate))
            .ToListAsync(cancellationToken);
    }
}
