using Rentify.Backend.Core.Domain.Commons;

namespace Rentify.Backend.Core.Domain.Entities.Vehicles;

public sealed class VehicleType : BaseEntity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;

    private VehicleType()
    {
    }

    private VehicleType(Guid id,string name,string createdBy)
    {
        Id = id;
        Name = name;
        CreatedBy = createdBy;
        ModifiedBy = createdBy;
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = CreatedDate;
        IsActive = true;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Vehicle type name is required.");

        if (name.Length > 100)
            throw new ArgumentException("Vehicle type name is too long.");
    }
}
