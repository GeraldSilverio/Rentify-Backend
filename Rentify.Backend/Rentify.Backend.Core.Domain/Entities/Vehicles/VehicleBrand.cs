using Rentify.Backend.Core.Domain.Commons;

namespace Rentify.Backend.Core.Domain.Entities.Vehicles;

public sealed class VehicleBrand : BaseEntity
{
    private readonly List<VehicleModel> _models = [];

    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;

    public IReadOnlyCollection<VehicleModel> Models => _models.AsReadOnly();

    private VehicleBrand()
    {
    }

    private VehicleBrand(Guid id, string name, string createdBy)
    {
        Id = id;
        Name = name;
        CreatedBy = createdBy;
        ModifiedBy = createdBy;
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = CreatedDate;
        IsActive = true;
    }

    public static VehicleBrand Create(string name, string createdBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Vehicle brand name is required.");

        if (name.Length > 100)
            throw new ArgumentException("Vehicle brand name is too long.");

        return new VehicleBrand(Guid.NewGuid(), name.Trim(), createdBy);
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
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Vehicle brand name is required.");

        if (name.Trim().Length > 100)
            throw new ArgumentException("Vehicle brand name is too long.");

        ValidateUser(user);
    }

    private static void ValidateUser(string user)
    {
        if (string.IsNullOrWhiteSpace(user))
            throw new ArgumentException("User is required.");
    }
}
