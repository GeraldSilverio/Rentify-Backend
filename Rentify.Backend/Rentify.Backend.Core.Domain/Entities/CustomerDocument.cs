using Rentify.Backend.Core.Domain.Commons;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Domain.Entities;

public sealed class CustomerDocument : BaseEntity
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid CustomerId { get; private set; }
    public string Name { get; private set; } = null!;
    public string Url { get; private set; } = null!;
    public string PublicId { get; private set; } = null!;
    public CustomerDocumentType DocumentType { get; private set; }
    public Customer Customer { get; private set; } = null!;

    private CustomerDocument()
    {
    }

    private CustomerDocument(
        Guid tenantId,
        Guid customerId,
        string name,
        string url,
        string publicId,
        CustomerDocumentType documentType,
        string createdBy)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        CustomerId = customerId;
        Name = name.Trim();
        Url = url.Trim();
        PublicId = publicId.Trim();
        DocumentType = documentType;
        CreatedBy = createdBy;
        ModifiedBy = createdBy;
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = CreatedDate;
        IsActive = true;
    }

    public static CustomerDocument Create(
        Guid tenantId,
        Guid customerId,
        string name,
        string url,
        string publicId,
        CustomerDocumentType documentType,
        string createdBy)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant Id is required.");

        if (customerId == Guid.Empty)
            throw new ArgumentException("Customer Id is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Document name is required.");

        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Document URL is required.");

        if (string.IsNullOrWhiteSpace(publicId))
            throw new ArgumentException("Document public Id is required.");

        return new CustomerDocument(tenantId, customerId, name, url, publicId, documentType, createdBy);
    }
}
