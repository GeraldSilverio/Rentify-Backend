using Rentify.Backend.Core.Domain.Commons;

namespace Rentify.Backend.Core.Domain.Entities;

public class Brand : BaseEntity
{
    private readonly List<Model> _models = [];

    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;

    public IReadOnlyCollection<Model> Models => _models.AsReadOnly();

    private Brand()
    {
    }

    private Brand(Guid id, string name, string createdBy)
    {
        Id = id;
        Name = name;
        CreatedBy = createdBy;
        ModifiedBy = createdBy;
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = CreatedDate;
        IsActive = true;
    }

    public static Brand Create(string name, string createdBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Brand name is required.");

        if (name.Length > 100)
            throw new ArgumentException("Brand name is too long.");

        return new Brand(Guid.NewGuid(), name.Trim(), createdBy);
    }
}
