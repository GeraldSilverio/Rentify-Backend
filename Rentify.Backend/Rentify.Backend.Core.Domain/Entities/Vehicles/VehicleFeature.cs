using Rentify.Backend.Core.Domain.Commons;

namespace Rentify.Backend.Core.Domain.Entities.Vehicles;

public sealed class VehicleFeature : BaseEntity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Category { get; private set; } = null!;

    private VehicleFeature()
    {
    }

    private VehicleFeature(Guid id, string name, string category, string createdBy)
    {
        Id = id;
        Name = Normalize(name);
        Category = Normalize(category);
        CreatedBy = createdBy;
        ModifiedBy = createdBy;
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = CreatedDate;
        IsActive = true;
        IsDeleted = false;
    }

    public static VehicleFeature Create(string name, string category, string createdBy)
    {
        Validate(name, category, createdBy);
        return new VehicleFeature(Guid.NewGuid(), name, category, createdBy);
    }

    public void Update(string name, string category, string modifiedBy)
    {
        Validate(name, category, modifiedBy);
        Name = Normalize(name);
        Category = Normalize(category);
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

    private static void Validate(string name, string category, string user)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Vehicle feature name is required.");

        if (name.Trim().Length > 100)
            throw new ArgumentException("Vehicle feature name is too long.");

        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Vehicle feature category is required.");

        if (category.Trim().Length > 100)
            throw new ArgumentException("Vehicle feature category is too long.");

        ValidateUser(user);
    }

    private static void ValidateUser(string user)
    {
        if (string.IsNullOrWhiteSpace(user))
            throw new ArgumentException("User is required.");
    }

    private static string Normalize(string value)
    {
        return value.Trim();
    }
}
