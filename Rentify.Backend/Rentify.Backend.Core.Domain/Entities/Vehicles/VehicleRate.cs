using Rentify.Backend.Core.Domain.Commons;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Domain.Entities.Vehicles;

public sealed class VehicleRate : BaseEntity
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid VehicleId { get; private set; }
    public RentalType RentalType { get; private set; }
    public decimal Price { get; private set; }

    public Vehicle Vehicle { get; private set; } = null!;

    private VehicleRate()
    {
    }

    private VehicleRate(
        Guid id,
        Guid tenantId,
        Guid vehicleId,
        RentalType rentalType,
        decimal price,
        string createdBy)
    {
        Id = id;
        TenantId = tenantId;
        VehicleId = vehicleId;
        RentalType = rentalType;
        Price = price;
        CreatedBy = createdBy;
        ModifiedBy = createdBy;
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = CreatedDate;
        IsActive = true;
        IsDeleted = false;
    }

    public static VehicleRate Create(
        Guid tenantId,
        Guid vehicleId,
        RentalType rentalType,
        decimal price,
        string createdBy)
    {
        Validate(tenantId, vehicleId, rentalType, price, createdBy);

        return new VehicleRate(
            Guid.NewGuid(),
            tenantId,
            vehicleId,
            rentalType,
            price,
            createdBy);
    }

    public void Update(decimal price, string modifiedBy)
    {
        if (price <= 0)
            throw new ArgumentException("Vehicle rate price must be greater than zero.");

        if (string.IsNullOrWhiteSpace(modifiedBy))
            throw new ArgumentException("ModifiedBy is required.");

        Price = price;
        ModifiedBy = modifiedBy;
        ModifiedDate = DateTime.UtcNow;
    }

    public void Delete(string modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(modifiedBy))
            throw new ArgumentException("ModifiedBy is required.");

        IsDeleted = true;
        IsActive = false;
        ModifiedBy = modifiedBy;
        ModifiedDate = DateTime.UtcNow;
    }

    private static void Validate(
        Guid tenantId,
        Guid vehicleId,
        RentalType rentalType,
        decimal price,
        string user)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant Id is required.");

        if (vehicleId == Guid.Empty)
            throw new ArgumentException("Vehicle Id is required.");

        if (!Enum.IsDefined(typeof(RentalType), rentalType))
            throw new ArgumentException("Rental type is invalid.");

        if (price <= 0)
            throw new ArgumentException("Vehicle rate price must be greater than zero.");

        if (string.IsNullOrWhiteSpace(user))
            throw new ArgumentException("User is required.");
    }
}
