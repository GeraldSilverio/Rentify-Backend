using Rentify.Backend.Core.Domain.Commons;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Domain.Entities.Customers;

public sealed class CustomerDocument : BaseEntity
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid CustomerId { get; private set; }
    public string Name { get; private set; } = null!;
    public string Url { get; private set; } = null!;
    public string PublicId { get; private set; } = null!;
    public CustomerDocumentType DocumentType { get; private set; }
    public DocumentSide DocumentSide { get; private set; }
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
        string createdBy,
        DocumentSide documentSide)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        CustomerId = customerId;
        Name = name.Trim();
        Url = url.Trim();
        PublicId = publicId.Trim();
        DocumentType = documentType;
        DocumentSide = documentSide;
        CreatedBy = createdBy.Trim();
        ModifiedBy = CreatedBy;
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
        string createdBy,
        DocumentSide documentSide = DocumentSide.NotApplicable)
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

        if (string.IsNullOrWhiteSpace(createdBy))
            throw new ArgumentException("Created by is required.");

        if (!Enum.IsDefined(documentType))
            throw new ArgumentException("Document type is invalid.");

        if (!Enum.IsDefined(documentSide))
            throw new ArgumentException("Document side is invalid.");

        if (documentType == CustomerDocumentType.IdentificationSelfie
            && documentSide != DocumentSide.NotApplicable)
        {
            throw new ArgumentException("La selfie con cédula no debe tener lado frontal o trasero.");
        }

        return new CustomerDocument(
            tenantId,
            customerId,
            name,
            url,
            publicId,
            documentType,
            createdBy,
            documentSide);
    }

    public void Delete(string modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(modifiedBy))
            throw new ArgumentException("Modified by is required.", nameof(modifiedBy));

        IsDeleted = true;
        IsActive = false;
        ModifiedBy = modifiedBy.Trim();
        ModifiedDate = DateTime.UtcNow;
    }
}
