using Rentify.Backend.Core.Domain.Commons;

namespace Rentify.Backend.Core.Domain.Entities.Vehicles;

public class VehicleUnavailableDate : BaseEntity
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid VehicleId { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public string Reason { get; private set; } = null!;

    public Vehicle Vehicle { get; private set; } = null!;

    private VehicleUnavailableDate()
    {
    }

    private VehicleUnavailableDate(
        Guid id,
        Guid tenantId,
        Guid vehicleId,
        DateOnly startDate,
        DateOnly endDate,
        string reason,
        string createdBy)
    {
        Id = id;
        TenantId = tenantId;
        VehicleId = vehicleId;
        StartDate = startDate;
        EndDate = endDate;
        Reason = reason;
        CreatedBy = createdBy;
        ModifiedBy = createdBy;
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = CreatedDate;
        IsActive = true;
    }

    public static VehicleUnavailableDate Create(
        Guid tenantId,
        Guid vehicleId,
        DateOnly startDate,
        DateOnly endDate,
        string? reason,
        string createdBy)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant Id is required.");

        if (vehicleId == Guid.Empty)
            throw new ArgumentException("Vehicle Id is required.");

        if (endDate < startDate)
            throw new ArgumentException("End date must be greater than or equal to start date.");

        return new VehicleUnavailableDate(
            Guid.NewGuid(),
            tenantId,
            vehicleId,
            startDate,
            endDate,
            reason?.Trim() ?? string.Empty,
            createdBy);
    }

    public bool Overlaps(DateOnly startDate, DateOnly endDate)
    {
        return StartDate <= endDate && startDate <= EndDate;
    }
}
