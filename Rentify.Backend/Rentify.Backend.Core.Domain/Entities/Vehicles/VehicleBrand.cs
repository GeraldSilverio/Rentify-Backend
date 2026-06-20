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
}
