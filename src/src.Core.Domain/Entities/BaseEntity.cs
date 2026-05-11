namespace Rentify.Backend.Core.Domain.Entities;

public class BaseEntity
{
    public Guid CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid UpdatedBy { get; set; }
    public DateTime UpdatedDate { get; set; }
    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; }

    public void SetActive()
    {
        IsActive = true;
        UpdatedDate = DateTime.UtcNow;
    }

    public void SetInactive()
    {
        IsActive = false;
        UpdatedDate = DateTime.UtcNow;
    }

    public void SetCreatedBy(Guid createdBy)
    {
        CreatedBy = createdBy;
        CreatedDate = DateTime.UtcNow;
    }

    public void SetUpdatedBy(Guid updatedBy)
    {
        UpdatedBy = updatedBy;
        UpdatedDate = DateTime.UtcNow;
    }

    public void SetDeletedBy(Guid updatedBy)
    {
        IsDeleted = true;
        UpdatedBy = updatedBy;
        UpdatedDate = DateTime.UtcNow;
    }
}