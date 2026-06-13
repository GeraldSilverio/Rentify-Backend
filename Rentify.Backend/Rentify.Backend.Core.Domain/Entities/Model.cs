using Rentify.Backend.Core.Domain.Commons;

namespace Rentify.Backend.Core.Domain.Entities;

public class Model : BaseEntity
{
    public Guid Id { get; private set; }
    public Guid BrandId { get; private set; }
    public string Name { get; private set; } = null!;

    public Brand Brand { get; private set; } = null!;

    private Model()
    {
    }

    private Model(Guid id, Guid brandId, string name, string createdBy)
    {
        Id = id;
        BrandId = brandId;
        Name = name;
        CreatedBy = createdBy;
        ModifiedBy = createdBy;
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = CreatedDate;
        IsActive = true;
    }

    public static Model Create(Guid brandId, string name, string createdBy)
    {
        if (brandId == Guid.Empty)
            throw new ArgumentException("Brand Id is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Model name is required.");

        if (name.Length > 100)
            throw new ArgumentException("Model name is too long.");

        return new Model(Guid.NewGuid(), brandId, name.Trim(), createdBy);
    }
}
