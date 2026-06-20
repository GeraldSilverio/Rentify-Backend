using Rentify.Backend.Core.Domain.Commons;

namespace Rentify.Backend.Core.Domain.Entities.Vehicles;

public sealed class VehicleType : BaseEntity
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    private VehicleType()
    {
    }

    private VehicleType(Guid id, Guid tenantId, string name, string? description, string createdBy)
    {
        Id = id;
        TenantId = tenantId;
        Name = name;
        Description = NormalizeDescription(description);
        CreatedBy = createdBy;
        ModifiedBy = createdBy;
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = CreatedDate;
        IsActive = true;
    }

    public static VehicleType Create(Guid tenantId, string name, string? description, string createdBy)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant Id is required.");

        ValidateName(name);

        return new VehicleType(Guid.NewGuid(), tenantId, name.Trim(), description, createdBy);
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Vehicle type name is required.");

        if (name.Length > 100)
            throw new ArgumentException("Vehicle type name is too long.");
    }

    private static string? NormalizeDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return null;

        if (description.Length > 300)
            throw new ArgumentException("Vehicle type description is too long.");

        return description.Trim();
    }
}
