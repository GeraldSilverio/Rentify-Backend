using Rentify.Backend.Core.Domain.Commons;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Domain.Entities.Core;

public sealed class Tenant : BaseEntity
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = null!;

    public string? LegalName { get; private set; }

    public string? Rnc { get; private set; }

    public BusinessModel BusinessModel { get; private set; }

    // EF
    private Tenant()
    {
    }

    private Tenant(
        Guid id,
        string name,
        string? legalName,
        string? rnc,
        BusinessModel businessModel,
        string createdBy)
    {
        Id = id;
        Name = NormalizeName(name);
        LegalName = string.IsNullOrWhiteSpace(legalName) ? null : legalName.Trim();
        Rnc = string.IsNullOrWhiteSpace(rnc) ? null : NormalizeRnc(rnc);
        BusinessModel = businessModel;

        IsActive = true;
        IsDeleted = false;
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = CreatedDate;
        CreatedBy = createdBy;
        ModifiedBy = createdBy;
    }

    public static Tenant Create(
        string name,
        string? legalName,
        string? rnc,
        BusinessModel businessModel,
        string createdBy)
    {
        Validate(name, businessModel, createdBy);

        return new Tenant(
            Guid.NewGuid(),
            name,
            legalName,
            rnc,
            businessModel,
            createdBy);
    }

    public void Update(
        string name,
        string? legalName,
        string? rnc,
        BusinessModel businessModel,
        string modifiedBy)
    {
        Validate(name, businessModel, modifiedBy);

        Name = NormalizeName(name);
        LegalName = string.IsNullOrWhiteSpace(legalName) ? null : legalName.Trim();
        Rnc = string.IsNullOrWhiteSpace(rnc) ? null : NormalizeRnc(rnc);
        BusinessModel = businessModel;

        ModifiedBy = modifiedBy;
        ModifiedDate = DateTime.UtcNow;
    }

    public void Suspend(string modifiedBy)
    {
        IsActive = false;
        ModifiedBy = modifiedBy;
        ModifiedDate = DateTime.UtcNow;
    }

    public void Activate(string modifiedBy)
    {
        IsActive = true;
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

    private static void Validate(string name, BusinessModel businessModel, string user)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tenant name is required.");

        if (name.Trim().Length > 150)
            throw new ArgumentException("Tenant name is too long.");

        if (!Enum.IsDefined(typeof(BusinessModel), businessModel))
            throw new ArgumentException("Invalid business model.");

        if (string.IsNullOrWhiteSpace(user))
            throw new ArgumentException("User is required.");
    }

    private static string NormalizeName(string name)
    {
        return name.Trim();
    }

    private static string NormalizeRnc(string rnc)
    {
        return rnc.Trim().Replace("-", string.Empty);
    }
}