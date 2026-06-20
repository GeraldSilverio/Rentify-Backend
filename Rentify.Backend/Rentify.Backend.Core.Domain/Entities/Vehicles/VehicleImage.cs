using Rentify.Backend.Core.Domain.Commons;

namespace Rentify.Backend.Core.Domain.Entities.Vehicles;

public sealed class VehicleImage : BaseEntity
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid VehicleId { get; private set; }
    public string Url { get; private set; } = null!;
    public string PublicId { get; private set; } = null!;
    public bool IsPrimary { get; private set; }

    public Vehicle Vehicle { get; private set; } = null!;

    private VehicleImage()
    {
    }

    private VehicleImage(
        Guid id,
        Guid tenantId,
        Guid vehicleId,
        string url,
        string publicId,
        bool isPrimary,
        string createdBy)
    {
        Id = id;
        TenantId = tenantId;
        VehicleId = vehicleId;
        Url = url;
        PublicId = publicId;
        IsPrimary = isPrimary;
        CreatedBy = createdBy;
        ModifiedBy = createdBy;
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = CreatedDate;
        IsActive = true;
    }

    public static VehicleImage Create(
        Guid tenantId,
        Guid vehicleId,
        string url,
        string publicId,
        bool isPrimary,
        string createdBy)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant Id is required.");

        if (vehicleId == Guid.Empty)
            throw new ArgumentException("Vehicle Id is required.");

        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Vehicle image URL is required.");

        if (string.IsNullOrWhiteSpace(publicId))
            throw new ArgumentException("Vehicle image public Id is required.");

        return new VehicleImage(
            Guid.NewGuid(),
            tenantId,
            vehicleId,
            url.Trim(),
            publicId.Trim(),
            isPrimary,
            createdBy);
    }

    public void MarkAsPrimary(string modifiedBy)
    {
        IsPrimary = true;
        ModifiedBy = modifiedBy;
        ModifiedDate = DateTime.UtcNow;
    }

    public void UnmarkAsPrimary(string modifiedBy)
    {
        IsPrimary = false;
        ModifiedBy = modifiedBy;
        ModifiedDate = DateTime.UtcNow;
    }
}
