using Rentify.Backend.Core.Domain.Commons;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Domain.Entities.Customers;

public sealed class Customer : BaseEntity
{
    private readonly List<CustomerDocument> _documents = [];

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public CustomerType CustomerType { get; private set; }
    public IdentificationType IdentificationType { get; private set; }
    public string IdentificationNumber { get; private set; } = null!;
    public string IdentificationNumberNormalized { get; private set; } = null!;
    public DateOnly? BirthDate { get; private set; }
    public bool IsVerified { get; private set; }
    public DateTime? VerifiedAt { get; private set; }
    public string? VerifiedBy { get; private set; }
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PhoneNumber { get; private set; } = null!;
    public string? AddressLine { get; private set; }
    public string? Sector { get; private set; }
    public string? City { get; private set; }
    public string? Province { get; private set; }
    public string? AddressReference { get; private set; }
    public IReadOnlyCollection<CustomerDocument> Documents => _documents.AsReadOnly();

    private Customer()
    {
    }

    private Customer(
        Guid tenantId,
        CustomerType customerType,
        IdentificationType identificationType,
        string identificationNumber,
        DateOnly? birthDate,
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        string? addressLine,
        string? sector,
        string? city,
        string? province,
        string? addressReference,
        string createdBy)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        CustomerType = customerType;
        IdentificationType = identificationType;
        IdentificationNumber = FormatIdentificationNumber(identificationType, identificationNumber);
        IdentificationNumberNormalized = NormalizeIdentificationNumber(identificationNumber);
        BirthDate = birthDate;
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email.Trim().ToLowerInvariant();
        PhoneNumber = phoneNumber.Trim();
        AddressLine = NormalizeOptionalText(addressLine);
        Sector = NormalizeOptionalText(sector);
        City = NormalizeOptionalText(city);
        Province = NormalizeOptionalText(province);
        AddressReference = NormalizeOptionalText(addressReference);
        CreatedBy = createdBy.Trim();
        ModifiedBy = CreatedBy;
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = CreatedDate;
        IsActive = true;
        IsVerified = false;
    }

    public static Customer Create(
        Guid tenantId,
        CustomerType customerType,
        IdentificationType identificationType,
        string identificationNumber,
        DateOnly? birthDate,
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        string? addressLine,
        string? sector,
        string? city,
        string? province,
        string? addressReference,
        string createdBy)
    {
        Validate(
            tenantId,
            customerType,
            identificationType,
            identificationNumber,
            firstName,
            lastName,
            email,
            phoneNumber,
            addressLine,
            sector,
            city,
            province,
            addressReference,
            createdBy);

        return new Customer(
            tenantId,
            customerType,
            identificationType,
            identificationNumber,
            birthDate,
            firstName,
            lastName,
            email,
            phoneNumber,
            addressLine,
            sector,
            city,
            province,
            addressReference,
            createdBy);
    }

    public void Update(
        CustomerType customerType,
        IdentificationType identificationType,
        string identificationNumber,
        DateOnly? birthDate,
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        string? addressLine,
        string? sector,
        string? city,
        string? province,
        string? addressReference,
        string modifiedBy)
    {
        Validate(
            TenantId,
            customerType,
            identificationType,
            identificationNumber,
            firstName,
            lastName,
            email,
            phoneNumber,
            addressLine,
            sector,
            city,
            province,
            addressReference,
            modifiedBy);

        CustomerType = customerType;
        IdentificationType = identificationType;
        IdentificationNumber = FormatIdentificationNumber(identificationType, identificationNumber);
        IdentificationNumberNormalized = NormalizeIdentificationNumber(identificationNumber);
        BirthDate = birthDate;
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email.Trim().ToLowerInvariant();
        PhoneNumber = phoneNumber.Trim();
        AddressLine = NormalizeOptionalText(addressLine);
        Sector = NormalizeOptionalText(sector);
        City = NormalizeOptionalText(city);
        Province = NormalizeOptionalText(province);
        AddressReference = NormalizeOptionalText(addressReference);
        ModifiedBy = modifiedBy.Trim();
        ModifiedDate = DateTime.UtcNow;
    }

    public void MarkAsVerified(string verifiedBy)
    {
        ValidateActor(verifiedBy, nameof(verifiedBy));
        IsVerified = true;
        VerifiedAt = DateTime.UtcNow;
        VerifiedBy = verifiedBy.Trim();
        ModifiedBy = VerifiedBy;
        ModifiedDate = DateTime.UtcNow;
    }

    public void MarkAsUnverified(string modifiedBy)
    {
        ValidateActor(modifiedBy, nameof(modifiedBy));
        IsVerified = false;
        VerifiedAt = null;
        VerifiedBy = null;
        ModifiedBy = modifiedBy.Trim();
        ModifiedDate = DateTime.UtcNow;
    }

    public CustomerDocument AddDocument(
        string name,
        string url,
        string publicId,
        CustomerDocumentType documentType,
        string createdBy,
        DocumentSide documentSide = DocumentSide.NotApplicable)
    {
        CustomerDocument document = CustomerDocument.Create(
            TenantId,
            Id,
            name,
            url,
            publicId,
            documentType,
            createdBy,
            documentSide);
        _documents.Add(document);
        ModifiedBy = createdBy.Trim();
        ModifiedDate = DateTime.UtcNow;
        return document;
    }

    public void Delete(string modifiedBy)
    {
        ValidateActor(modifiedBy, nameof(modifiedBy));
        IsDeleted = true;
        IsActive = false;
        ModifiedBy = modifiedBy.Trim();
        ModifiedDate = DateTime.UtcNow;
    }

    public static string NormalizeIdentificationNumber(string identificationNumber)
    {
        if (string.IsNullOrWhiteSpace(identificationNumber))
            throw new ArgumentException("Identification number is required.", nameof(identificationNumber));

        return new string(identificationNumber
            .Trim()
            .Where(character => character != '-' && !char.IsWhiteSpace(character))
            .ToArray())
            .ToUpperInvariant();
    }

    public static string FormatIdentificationNumber(
        IdentificationType identificationType,
        string identificationNumber)
    {
        string normalized = NormalizeIdentificationNumber(identificationNumber);

        return identificationType switch
        {
            IdentificationType.Cedula when normalized.Length == 11 && normalized.All(char.IsDigit)
                => $"{normalized[..3]}-{normalized.Substring(3, 7)}-{normalized[^1]}",
            IdentificationType.Cedula => normalized,
            IdentificationType.Passport => normalized,
            _ => throw new ArgumentException("Identification type is invalid.", nameof(identificationType))
        };
    }

    private static void Validate(
        Guid tenantId,
        CustomerType customerType,
        IdentificationType identificationType,
        string identificationNumber,
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        string? addressLine,
        string? sector,
        string? city,
        string? province,
        string? addressReference,
        string actor)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant Id is required.");

        if (!Enum.IsDefined(customerType))
            throw new ArgumentException("Customer type is invalid.");

        if (!Enum.IsDefined(identificationType))
            throw new ArgumentException("Identification type is invalid.");

        NormalizeIdentificationNumber(identificationNumber);

        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required.");

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.");

        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number is required.");

        ValidateOptionalText(addressLine, 250, nameof(addressLine));
        ValidateOptionalText(sector, 100, nameof(sector));
        ValidateOptionalText(city, 100, nameof(city));
        ValidateOptionalText(province, 100, nameof(province));
        ValidateOptionalText(addressReference, 250, nameof(addressReference));

        ValidateActor(actor, nameof(actor));
    }

    private static void ValidateActor(string actor, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(actor))
            throw new ArgumentException("Actor is required.", parameterName);
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static void ValidateOptionalText(string? value, int maximumLength, string parameterName)
    {
        string? normalizedValue = NormalizeOptionalText(value);
        if (normalizedValue is not null && normalizedValue.Length > maximumLength)
            throw new ArgumentException($"{parameterName} cannot exceed {maximumLength} characters.", parameterName);
    }
}
