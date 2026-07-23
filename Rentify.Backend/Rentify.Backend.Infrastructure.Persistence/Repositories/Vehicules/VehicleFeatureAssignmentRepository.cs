using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Domain.Entities.Vehicles;
using Rentify.Backend.Infrastructure.Persistence.Context;

namespace Rentify.Backend.Infrastructure.Persistence.Repositories.Vehicules;

public sealed class VehicleFeatureAssignmentRepository : IVehicleFeatureAssignmentRepository
{
    private readonly RentifyContext _context;

    public VehicleFeatureAssignmentRepository(RentifyContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<VehicleFeatureAssignment>> GetAssignmentsByVehicleAsync(
        Guid tenantId,
        Guid vehicleId,
        CancellationToken cancellationToken = default)
    {
        return await _context.VehicleFeatureAssignments
            .Where(assignment => assignment.TenantId == tenantId && assignment.VehicleId == vehicleId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<VehicleFeatureAssignment>> GetAssignmentsByVehicleWithFeaturesAsync(
        Guid tenantId,
        Guid vehicleId,
        CancellationToken cancellationToken = default)
    {
        return await _context.VehicleFeatureAssignments
            .AsNoTracking()
            .Include(assignment => assignment.VehicleFeature)
            .Where(assignment => assignment.TenantId == tenantId && assignment.VehicleId == vehicleId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddRangeAsync(
        IReadOnlyCollection<VehicleFeatureAssignment> assignments,
        CancellationToken cancellationToken = default)
    {
        await _context.VehicleFeatureAssignments.AddRangeAsync(assignments, cancellationToken);
    }

    public void RemoveRange(IReadOnlyCollection<VehicleFeatureAssignment> assignments)
    {
        _context.VehicleFeatureAssignments.RemoveRange(assignments);
    }
}
