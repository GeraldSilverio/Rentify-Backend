using Rentify.Backend.Core.Domain.Commons;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Domain.Entities.Locations;

public sealed class Location : BaseEntity
{
    public const string DefaultCountry = "República Dominicana";

    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public LocationType Type { get; private set; }
    public string? City { get; private set; }
    public string? Province { get; private set; }
    public string Country { get; private set; } = DefaultCountry;
    public string? Address { get; private set; }
    public string? Notes { get; private set; }

    private Location()
    {
    }

    private Location(
        Guid id,
        string name,
        LocationType type,
        string? city,
        string? province,
        string? country,
        string? address,
        string? notes,
        string createdBy)
    {
        Validate(name, type, city, province, country, address, notes, createdBy);

        Id = id;
        Name = NormalizeRequired(name);
        Type = type;
        City = NormalizeOptional(city);
        Province = NormalizeOptional(province);
        Country = NormalizeCountry(country);
        Address = NormalizeOptional(address);
        Notes = NormalizeOptional(notes);
        CreatedBy = createdBy.Trim();
        ModifiedBy = CreatedBy;
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = CreatedDate;
        IsActive = true;
        IsDeleted = false;
    }

    public static Location Create(
        string name,
        LocationType type,
        string? city,
        string? province,
        string? country,
        string? address,
        string? notes,
        string createdBy)
    {
        return new Location(Guid.NewGuid(), name, type, city, province, country, address, notes, createdBy);
    }

    public void Update(
        string name,
        LocationType type,
        string? city,
        string? province,
        string? country,
        string? address,
        string? notes,
        string modifiedBy)
    {
        Validate(name, type, city, province, country, address, notes, modifiedBy);

        Name = NormalizeRequired(name);
        Type = type;
        City = NormalizeOptional(city);
        Province = NormalizeOptional(province);
        Country = NormalizeCountry(country);
        Address = NormalizeOptional(address);
        Notes = NormalizeOptional(notes);
        ModifiedBy = modifiedBy.Trim();
        ModifiedDate = DateTime.UtcNow;
    }

    public void Activate(string modifiedBy)
    {
        SetAudit(modifiedBy);
        IsActive = true;
        IsDeleted = false;
    }

    public void Deactivate(string modifiedBy)
    {
        SetAudit(modifiedBy);
        IsActive = false;
    }

    public void Delete(string modifiedBy)
    {
        SetAudit(modifiedBy);
        IsDeleted = true;
        IsActive = false;
    }

    private static void Validate(
        string name,
        LocationType type,
        string? city,
        string? province,
        string? country,
        string? address,
        string? notes,
        string user)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Location name is required.");

        if (name.Trim().Length > 150)
            throw new ArgumentException("Location name is too long.");

        if (!Enum.IsDefined(typeof(LocationType), type))
            throw new ArgumentException("Location type is invalid.");

        ValidateOptionalLength(city, 100, "City is too long.");
        ValidateOptionalLength(province, 100, "Province is too long.");

        string normalizedCountry = NormalizeCountry(country);
        if (normalizedCountry.Length > 100)
            throw new ArgumentException("Country is too long.");

        ValidateOptionalLength(address, 250, "Address is too long.");
        ValidateOptionalLength(notes, 500, "Notes is too long.");

        if (string.IsNullOrWhiteSpace(user))
            throw new ArgumentException("User is required.");
    }

    private void SetAudit(string modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(modifiedBy))
            throw new ArgumentException("ModifiedBy is required.");

        ModifiedBy = modifiedBy.Trim();
        ModifiedDate = DateTime.UtcNow;
    }

    private static void ValidateOptionalLength(string? value, int maxLength, string message)
    {
        if (!string.IsNullOrWhiteSpace(value) && value.Trim().Length > maxLength)
            throw new ArgumentException(message);
    }

    private static string NormalizeRequired(string value)
    {
        return value.Trim();
    }

    private static string NormalizeCountry(string? country)
    {
        return string.IsNullOrWhiteSpace(country) ? DefaultCountry : country.Trim();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
