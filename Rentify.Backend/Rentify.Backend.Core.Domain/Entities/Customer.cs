using Rentify.Backend.Core.Domain.Commons;

namespace Rentify.Backend.Core.Domain.Entities;

public sealed class Customer : BaseEntity
{
    private readonly List<CustomerDocument> _documents = [];

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PhoneNumber { get; private set; } = null!;
    public string LicenseNumber { get; private set; } = null!;
    public DateOnly LicenseExpirationDate { get; private set; }
    public IReadOnlyCollection<CustomerDocument> Documents => _documents.AsReadOnly();

    private Customer()
    {
    }

    private Customer(
        Guid tenantId,
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        string licenseNumber,
        DateOnly licenseExpirationDate,
        string createdBy)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email.Trim().ToLowerInvariant();
        PhoneNumber = phoneNumber.Trim();
        LicenseNumber = NormalizeLicenseNumber(licenseNumber);
        LicenseExpirationDate = licenseExpirationDate;
        CreatedBy = createdBy;
        ModifiedBy = createdBy;
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = CreatedDate;
        IsActive = true;
    }

    public static Customer Create(
        Guid tenantId,
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        string licenseNumber,
        DateOnly licenseExpirationDate,
        string createdBy)
    {
        Validate(tenantId, firstName, lastName, email, phoneNumber, licenseNumber, licenseExpirationDate);

        return new Customer(
            tenantId,
            firstName,
            lastName,
            email,
            phoneNumber,
            licenseNumber,
            licenseExpirationDate,
            createdBy);
    }

    public void Update(
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        string licenseNumber,
        DateOnly licenseExpirationDate,
        string modifiedBy)
    {
        Validate(TenantId, firstName, lastName, email, phoneNumber, licenseNumber, licenseExpirationDate);

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email.Trim().ToLowerInvariant();
        PhoneNumber = phoneNumber.Trim();
        LicenseNumber = NormalizeLicenseNumber(licenseNumber);
        LicenseExpirationDate = licenseExpirationDate;
        ModifiedBy = modifiedBy;
        ModifiedDate = DateTime.UtcNow;
    }

    public CustomerDocument AddDocument(
        string name,
        string url,
        string publicId,
        Enums.CustomerDocumentType documentType,
        string createdBy)
    {
        CustomerDocument document = CustomerDocument.Create(TenantId, Id, name, url, publicId, documentType, createdBy);
        _documents.Add(document);
        ModifiedBy = createdBy;
        ModifiedDate = DateTime.UtcNow;
        return document;
    }

    public void Delete(string modifiedBy)
    {
        IsDeleted = true;
        IsActive = false;
        ModifiedBy = modifiedBy;
        ModifiedDate = DateTime.UtcNow;
    }

    public static string NormalizeLicenseNumber(string licenseNumber)
    {
        return licenseNumber.Trim().ToUpperInvariant().Replace("-", string.Empty).Replace(" ", string.Empty);
    }

    private static void Validate(
        Guid tenantId,
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        string licenseNumber,
        DateOnly licenseExpirationDate)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant Id is required.");

        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required.");

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.");

        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number is required.");

        if (string.IsNullOrWhiteSpace(licenseNumber))
            throw new ArgumentException("License number is required.");

        if (licenseExpirationDate <= DateOnly.FromDateTime(DateTime.UtcNow))
            throw new ArgumentException("License expiration date must be in the future.");
    }
}
