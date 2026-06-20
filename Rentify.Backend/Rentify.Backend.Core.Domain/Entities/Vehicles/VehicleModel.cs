using Rentify.Backend.Core.Domain.Commons;

namespace Rentify.Backend.Core.Domain.Entities.Vehicles;

public sealed class VehicleModel : BaseEntity
{
    public Guid Id { get; private set; }
    public Guid VehicleBrandId { get; private set; }
    public string Name { get; private set; } = null!;

    public VehicleBrand VehicleBrand { get; private set; } = null!;

    private VehicleModel()
    {
    }

    private VehicleModel(Guid id, Guid vehicleBrandId, string name, string createdBy)
    {
        Id = id;
        VehicleBrandId = vehicleBrandId;
        Name = name;
        CreatedBy = createdBy;
        ModifiedBy = createdBy;
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = CreatedDate;
        IsActive = true;
    }

    public static VehicleModel Create(Guid vehicleBrandId, string name, string createdBy)
    {
        if (vehicleBrandId == Guid.Empty)
            throw new ArgumentException("Vehicle brand Id is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Vehicle model name is required.");

        if (name.Length > 100)
            throw new ArgumentException("Vehicle model name is too long.");

        return new VehicleModel(Guid.NewGuid(), vehicleBrandId, name.Trim(), createdBy);
    }
}
