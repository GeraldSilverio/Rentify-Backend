using Rentify.Backend.Core.Domain.Commons;

namespace Rentify.Backend.Core.Domain.Entities;

public class VehicleImage : BaseEntity
{
    public Guid Id { get; private set; }
    public Guid VehicleId { get; private set; }
    public string Url { get; private set; } = null!;
    public string FileName { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    public long SizeInBytes { get; private set; }
    public bool IsPrimary { get; private set; }

    public Vehicle Vehicle { get; private set; } = null!;

    private VehicleImage()
    {
    }

    private VehicleImage(
        Guid id,
        Guid vehicleId,
        string url,
        string fileName,
        string contentType,
        long sizeInBytes,
        bool isPrimary,
        string createdBy)
    {
        Id = id;
        VehicleId = vehicleId;
        Url = url;
        FileName = fileName;
        ContentType = contentType;
        SizeInBytes = sizeInBytes;
        IsPrimary = isPrimary;
        CreatedBy = createdBy;
        ModifiedBy = createdBy;
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = CreatedDate;
        IsActive = true;
    }

    public static VehicleImage Create(
        Guid vehicleId,
        string url,
        string fileName,
        string contentType,
        long sizeInBytes,
        bool isPrimary,
        string createdBy)
    {
        if (vehicleId == Guid.Empty)
            throw new ArgumentException("Vehicle Id is required.");

        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Image URL is required.");

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name is required.");

        if (string.IsNullOrWhiteSpace(contentType))
            throw new ArgumentException("Content type is required.");

        if (sizeInBytes <= 0)
            throw new ArgumentException("Image size must be greater than zero.");

        return new VehicleImage(
            Guid.NewGuid(),
            vehicleId,
            url.Trim(),
            fileName.Trim(),
            contentType.Trim(),
            sizeInBytes,
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

    public void Delete(string modifiedBy)
    {
        IsDeleted = true;
        IsActive = false;
        ModifiedBy = modifiedBy;
        ModifiedDate = DateTime.UtcNow;
    }
}
