using Rentify.Backend.Core.Domain.Commons;
using Rentify.Backend.Core.Domain.Entities.Core;

namespace Rentify.Backend.Core.Domain.Entities.Locations;

public sealed class TenantLocation : BaseEntity
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid? LocationId { get; private set; }
    public string DisplayName { get; private set; } = null!;
    public bool AllowsDelivery { get; private set; }
    public bool AllowsPickup { get; private set; }
    public decimal DeliveryFee { get; private set; }
    public decimal PickupFee { get; private set; }
    public bool IsCustom { get; private set; }
    public Tenant Tenant { get; private set; } = null!;
    public Location? Location { get; private set; }

    private TenantLocation()
    {
    }

    private TenantLocation(
        Guid id,
        Guid tenantId,
        Guid? locationId,
        string displayName,
        bool allowsDelivery,
        bool allowsPickup,
        decimal deliveryFee,
        decimal pickupFee,
        bool isCustom,
        string createdBy)
    {
        Validate(tenantId, locationId, displayName, allowsDelivery, allowsPickup, deliveryFee, pickupFee, isCustom, createdBy);

        Id = id;
        TenantId = tenantId;
        LocationId = locationId;
        DisplayName = displayName.Trim();
        AllowsDelivery = allowsDelivery;
        AllowsPickup = allowsPickup;
        DeliveryFee = allowsDelivery ? deliveryFee : 0;
        PickupFee = allowsPickup ? pickupFee : 0;
        IsCustom = isCustom;
        CreatedBy = createdBy.Trim();
        ModifiedBy = CreatedBy;
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = CreatedDate;
        IsActive = true;
        IsDeleted = false;
    }

    public static TenantLocation CreateFromGlobalLocation(
        Guid tenantId,
        Guid locationId,
        string displayName,
        bool allowsDelivery,
        bool allowsPickup,
        decimal deliveryFee,
        decimal pickupFee,
        string createdBy)
    {
        return new TenantLocation(
            Guid.NewGuid(),
            tenantId,
            locationId,
            displayName,
            allowsDelivery,
            allowsPickup,
            deliveryFee,
            pickupFee,
            false,
            createdBy);
    }

    public static TenantLocation CreateCustom(
        Guid tenantId,
        string displayName,
        bool allowsDelivery,
        bool allowsPickup,
        decimal deliveryFee,
        decimal pickupFee,
        string createdBy)
    {
        return new TenantLocation(
            Guid.NewGuid(),
            tenantId,
            null,
            displayName,
            allowsDelivery,
            allowsPickup,
            deliveryFee,
            pickupFee,
            true,
            createdBy);
    }

    public void Update(
        string displayName,
        bool allowsDelivery,
        bool allowsPickup,
        decimal deliveryFee,
        decimal pickupFee,
        string modifiedBy)
    {
        Validate(TenantId, LocationId, displayName, allowsDelivery, allowsPickup, deliveryFee, pickupFee, IsCustom, modifiedBy);

        DisplayName = displayName.Trim();
        AllowsDelivery = allowsDelivery;
        AllowsPickup = allowsPickup;
        DeliveryFee = allowsDelivery ? deliveryFee : 0;
        PickupFee = allowsPickup ? pickupFee : 0;
        ModifiedBy = modifiedBy.Trim();
        ModifiedDate = DateTime.UtcNow;
    }

    public void Activate(string modifiedBy)
    {
        SetAudit(modifiedBy);
        IsActive = true;
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
        Guid tenantId,
        Guid? locationId,
        string displayName,
        bool allowsDelivery,
        bool allowsPickup,
        decimal deliveryFee,
        decimal pickupFee,
        bool isCustom,
        string user)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant Id is required.");

        if (!isCustom && (!locationId.HasValue || locationId.Value == Guid.Empty))
            throw new ArgumentException("Location Id is required.");

        if (isCustom && locationId.HasValue)
            throw new ArgumentException("Custom tenant locations cannot reference a global location.");

        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("Display name is required.");

        if (displayName.Trim().Length > 150)
            throw new ArgumentException("Display name is too long.");

        if (!allowsDelivery && !allowsPickup)
            throw new ArgumentException("Tenant location must allow delivery or pickup.");

        if (deliveryFee < 0)
            throw new ArgumentException("Delivery fee cannot be negative.");

        if (pickupFee < 0)
            throw new ArgumentException("Pickup fee cannot be negative.");

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
}
