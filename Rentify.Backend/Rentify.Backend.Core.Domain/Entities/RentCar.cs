using Rentify.Backend.Core.Domain.Commons;
using Rentify.Backend.Core.Domain.ValueObjects;

namespace Rentify.Backend.Core.Domain.Entities;

public class RentCar : BaseEntity
{
    public Guid Id { get; private set; }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public string? LogoUrl { get; private set; }
    
    public Guid TenantId { get; private set; } = Guid.NewGuid();

    // Contacts
    public PhoneNumber Phone { get; private set; }
    public Email Email { get; private set; }
    public PhoneNumber WhatsApp { get; private set; }

    // Location
    public Address Address { get; private set; }

    // EF Core
    private RentCar()
    {
    }

    private RentCar(
        Guid id,
        string name,
        string description,
        PhoneNumber phone,
        Email email,
        PhoneNumber whatsApp,
        Address address,
        string? logoUrl,
        string createdBy,
        Guid tenantId)
    {
        Id = id;
        Name = name;
        Description = description;
        Phone = phone;
        Email = email;
        WhatsApp = whatsApp;
        Address = address;
        LogoUrl = NormalizeLogoUrl(logoUrl);
        IsActive = true;
        CreatedDate = DateTime.UtcNow;
        CreatedBy = createdBy;
        ModifiedBy = createdBy;
        ModifiedDate = CreatedDate;
        TenantId = tenantId;
    }

    public static RentCar Create(
        string name,
        string description,
        string phone,
        string email,
        string whatsApp,
        string street,
        string city,
        string country,
        string? logoUrl,
        string createdBy,
        Guid tenantId)
    {
        Validate(name);

        return new RentCar(
            Guid.NewGuid(),
            name.Trim(),
            description?.Trim() ?? string.Empty,
            new PhoneNumber(phone),
            new Email(email),
            new PhoneNumber(whatsApp),
            new Address(street, city, country),
            logoUrl,
            createdBy,
            tenantId);
    }

    public void Disable()
    {
        IsActive = false;
    }

    public void Enable()
    {
        IsActive = true;
    }


    public void Update(
        string name,
        string description,
        string phone,
        string email,
        string whatsApp,
        string street,
        string city,
        string country,
        string? logoUrl,
        string updatedBy)
    {
        Validate(name);

        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        Phone = new PhoneNumber(phone);
        Email = new Email(email);
        WhatsApp = new PhoneNumber(whatsApp);
        Address = new Address(street, city, country);
        LogoUrl = NormalizeLogoUrl(logoUrl);
        ModifiedBy = updatedBy;
        ModifiedDate = DateTime.UtcNow;
    }

    public void SetTenantId(Guid tenantId)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException(
                "TenantId cannot be empty.");

        TenantId = tenantId;
    }

    private static void Validate(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.");

        if (name.Length > 100)
            throw new ArgumentException("Name is too long.");
    }

    private static string? NormalizeLogoUrl(string? logoUrl)
    {
        if (string.IsNullOrWhiteSpace(logoUrl))
            return null;

        string normalizedLogoUrl = logoUrl.Trim();

        if (normalizedLogoUrl.Length > 500)
            throw new ArgumentException("Logo URL is too long.");

        return normalizedLogoUrl;
    }
}
