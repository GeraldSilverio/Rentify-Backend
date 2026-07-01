using Rentify.Backend.Core.Domain.Commons;

namespace Rentify.Backend.Core.Domain.Entities.Vehicles;

public sealed class VehicleFeatureAssignment : BaseEntity
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid VehicleId { get; private set; }
    public Guid VehicleFeatureId { get; private set; }

    public Vehicle Vehicle { get; private set; } = null!;
    public VehicleFeature VehicleFeature { get; private set; } = null!;

    private VehicleFeatureAssignment()
    {
    }

    private VehicleFeatureAssignment(
        Guid id,
        Guid tenantId,
        Guid vehicleId,
        Guid vehicleFeatureId,
        string createdBy)
    {
        Id = id;
        TenantId = tenantId;
        VehicleId = vehicleId;
        VehicleFeatureId = vehicleFeatureId;
        CreatedBy = createdBy;
        ModifiedBy = createdBy;
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = CreatedDate;
        IsActive = true;
        IsDeleted = false;
    }

    public static VehicleFeatureAssignment Create(
        Guid tenantId,
        Guid vehicleId,
        Guid vehicleFeatureId,
        string createdBy)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant Id is required.");

        if (vehicleId == Guid.Empty)
            throw new ArgumentException("Vehicle Id is required.");

        if (vehicleFeatureId == Guid.Empty)
            throw new ArgumentException("Vehicle feature Id is required.");

        if (string.IsNullOrWhiteSpace(createdBy))
            throw new ArgumentException("CreatedBy is required.");

        return new VehicleFeatureAssignment(Guid.NewGuid(), tenantId, vehicleId, vehicleFeatureId, createdBy);
    }

    public void Activate(string modifiedBy)
    {
        IsActive = true;
        IsDeleted = false;
        ModifiedBy = modifiedBy;
        ModifiedDate = DateTime.UtcNow;
    }

    public void Delete(string modifiedBy)
    {
        IsActive = false;
        IsDeleted = true;
        ModifiedBy = modifiedBy;
        ModifiedDate = DateTime.UtcNow;
    }
}
