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

    public static VehicleType Create(string name, string createdBy)
    {
        Validate(name, createdBy);
        return new VehicleType(Guid.NewGuid(), name.Trim(), createdBy);
    }

    public void Update(string name, string modifiedBy)
    {
        Validate(name, modifiedBy);
        Name = name.Trim();
        ModifiedBy = modifiedBy;
        ModifiedDate = DateTime.UtcNow;
    }

    public void Activate(string modifiedBy)
    {
        ValidateUser(modifiedBy);
        IsActive = true;
        ModifiedBy = modifiedBy;
        ModifiedDate = DateTime.UtcNow;
    }

    public void Deactivate(string modifiedBy)
    {
        ValidateUser(modifiedBy);
        IsActive = false;
        ModifiedBy = modifiedBy;
        ModifiedDate = DateTime.UtcNow;
    }

    private static void Validate(string name, string user)
    {
        ValidateName(name);
        ValidateUser(user);
    }

    private static void ValidateUser(string user)
    {
        if (string.IsNullOrWhiteSpace(user))
            throw new ArgumentException("User is required.");
    }
}
