using Rentify.Backend.Core.Domain.Commons;
using Rentify.Backend.Core.Domain.Entities.Customers;
using Rentify.Backend.Core.Domain.Entities.Vehicles;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Domain.Entities.Reservations;

public sealed class Reservation : BaseEntity
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string Code { get; private set; } = null!;
    public Guid CustomerId { get; private set; }
    public Guid VehicleId { get; private set; }
    public DateTime DeliveryDateTime { get; private set; }
    public DateTime ExpectedReturnDateTime { get; private set; }
    public RentalType RentalType { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitRate { get; private set; }
    public decimal RentalAmount { get; private set; }
    public bool SecurityDepositRequired { get; private set; }
    public decimal SecurityDepositAmount { get; private set; }
    public Guid? DeliveryTenantLocationId { get; private set; }
    public string DeliveryLocationName { get; private set; } = null!;
    public string? DeliveryAddressDetails { get; private set; }
    public decimal DeliveryFee { get; private set; }
    public Guid? ReturnTenantLocationId { get; private set; }
    public string ReturnLocationName { get; private set; } = null!;
    public string? ReturnAddressDetails { get; private set; }
    public decimal ReturnFee { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string? Notes { get; private set; }
    public ReservationStatus Status { get; private set; }
    public ReservationChannel Channel { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public string? ApprovedBy { get; private set; }
    public DateTime? RejectedAt { get; private set; }
    public string? RejectionReason { get; private set; }
    public string? RejectedBy { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public string? CancellationReason { get; private set; }
    public string? CancelledBy { get; private set; }
    public DateTime? ConvertedToRentalAt { get; private set; }
    public string? ConvertedToRentalBy { get; private set; }
    public Customer Customer { get; private set; } = null!;
    public Vehicle Vehicle { get; private set; } = null!;

    private Reservation()
    {
    }

    public static Reservation Create(
        Guid tenantId,
        string code,
        Guid customerId,
        Guid vehicleId,
        DateTime deliveryDateTime,
        DateTime expectedReturnDateTime,
        RentalType rentalType,
        int quantity,
        decimal unitRate,
        bool securityDepositRequired,
        decimal securityDepositAmount,
        Guid? deliveryTenantLocationId,
        string deliveryLocationName,
        string? deliveryAddressDetails,
        decimal deliveryFee,
        Guid? returnTenantLocationId,
        string returnLocationName,
        string? returnAddressDetails,
        decimal returnFee,
        decimal discountAmount,
        ReservationChannel channel,
        string? notes,
        string createdBy)
    {
        Validate(
            tenantId,
            code,
            customerId,
            vehicleId,
            deliveryDateTime,
            expectedReturnDateTime,
            rentalType,
            quantity,
            unitRate,
            securityDepositRequired,
            securityDepositAmount,
            deliveryLocationName,
            deliveryAddressDetails,
            deliveryFee,
            returnLocationName,
            returnAddressDetails,
            returnFee,
            discountAmount,
            channel,
            notes,
            createdBy);

        decimal normalizedDeposit = securityDepositRequired ? securityDepositAmount : 0;
        decimal rentalAmount = unitRate * quantity;
        DateTime now = DateTime.UtcNow;

        return new Reservation
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Code = code.Trim(),
            CustomerId = customerId,
            VehicleId = vehicleId,
            DeliveryDateTime = deliveryDateTime,
            ExpectedReturnDateTime = expectedReturnDateTime,
            RentalType = rentalType,
            Quantity = quantity,
            UnitRate = unitRate,
            RentalAmount = rentalAmount,
            SecurityDepositRequired = securityDepositRequired,
            SecurityDepositAmount = normalizedDeposit,
            DeliveryTenantLocationId = deliveryTenantLocationId,
            DeliveryLocationName = deliveryLocationName.Trim(),
            DeliveryAddressDetails = Normalize(deliveryAddressDetails),
            DeliveryFee = deliveryFee,
            ReturnTenantLocationId = returnTenantLocationId,
            ReturnLocationName = returnLocationName.Trim(),
            ReturnAddressDetails = Normalize(returnAddressDetails),
            ReturnFee = returnFee,
            DiscountAmount = discountAmount,
            TotalAmount = CalculateTotal(
                rentalAmount,
                normalizedDeposit,
                deliveryFee,
                returnFee,
                discountAmount),
            Channel = channel,
            Notes = Normalize(notes),
            Status = ReservationStatus.Pending,
            CreatedBy = createdBy.Trim(),
            ModifiedBy = createdBy.Trim(),
            CreatedDate = now,
            ModifiedDate = now,
            IsActive = true,
            IsDeleted = false
        };
    }

    public void UpdatePendingReservation(
        Guid customerId,
        Guid vehicleId,
        DateTime deliveryDateTime,
        DateTime expectedReturnDateTime,
        RentalType rentalType,
        int quantity,
        decimal unitRate,
        bool securityDepositRequired,
        decimal securityDepositAmount,
        Guid? deliveryTenantLocationId,
        string deliveryLocationName,
        string? deliveryAddressDetails,
        decimal deliveryFee,
        Guid? returnTenantLocationId,
        string returnLocationName,
        string? returnAddressDetails,
        decimal returnFee,
        decimal discountAmount,
        ReservationChannel channel,
        string? notes,
        string modifiedBy)
    {
        EnsurePending();

        Validate(
            TenantId,
            Code,
            CustomerId,
            VehicleId,
            deliveryDateTime,
            expectedReturnDateTime,
            rentalType,
            quantity,
            unitRate,
            securityDepositRequired,
            securityDepositAmount,
            deliveryLocationName,
            deliveryAddressDetails,
            deliveryFee,
            returnLocationName,
            returnAddressDetails,
            returnFee,
            discountAmount,
            channel,
            notes,
            modifiedBy);

        DeliveryDateTime = deliveryDateTime;
        ExpectedReturnDateTime = expectedReturnDateTime;
        RentalType = rentalType;
        Quantity = quantity;
        UnitRate = unitRate;
        RentalAmount = unitRate * quantity;
        SecurityDepositRequired = securityDepositRequired;
        SecurityDepositAmount = securityDepositRequired ? securityDepositAmount : 0;
        DeliveryTenantLocationId = deliveryTenantLocationId;
        DeliveryLocationName = deliveryLocationName.Trim();
        DeliveryAddressDetails = Normalize(deliveryAddressDetails);
        DeliveryFee = deliveryFee;
        ReturnTenantLocationId = returnTenantLocationId;
        ReturnLocationName = returnLocationName.Trim();
        ReturnAddressDetails = Normalize(returnAddressDetails);
        ReturnFee = returnFee;
        DiscountAmount = discountAmount;
        TotalAmount = CalculateTotal(
            RentalAmount,
            SecurityDepositAmount,
            DeliveryFee,
            ReturnFee,
            DiscountAmount);
        Channel = channel;
        Notes = Normalize(notes);

        SetAudit(modifiedBy);
    }

    public void Approve(string approvedBy)
    {
        EnsurePending();
        SetActor(approvedBy, out string actor);

        Status = ReservationStatus.Approved;
        ApprovedAt = DateTime.UtcNow;
        ApprovedBy = actor;

        SetAudit(actor);
    }

    public void Reject(
        string reason,
        string rejectedBy)
    {
        EnsurePending();
        ValidateReason(reason);
        SetActor(rejectedBy, out string actor);

        Status = ReservationStatus.Rejected;
        RejectedAt = DateTime.UtcNow;
        RejectedBy = actor;
        RejectionReason = reason.Trim();

        SetAudit(actor);
    }

    public void Cancel(
        string reason,
        string cancelledBy)
    {
        if (Status is not (ReservationStatus.Pending or ReservationStatus.Approved))
        {
            throw new InvalidOperationException("Only pending or approved reservations can be cancelled.");
        }

        ValidateReason(reason);
        SetActor(cancelledBy, out string actor);

        Status = ReservationStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        CancelledBy = actor;
        CancellationReason = reason.Trim();

        SetAudit(actor);
    }

    public void MarkAsConvertedToRental(string convertedBy)
    {
        if (Status != ReservationStatus.Approved)
        {
            throw new InvalidOperationException("Only approved reservations can be converted to rental.");
        }

        SetActor(convertedBy, out string actor);

        Status = ReservationStatus.ConvertedToRental;
        ConvertedToRentalAt = DateTime.UtcNow;
        ConvertedToRentalBy = actor;

        SetAudit(actor);
    }

    public void Delete(string modifiedBy)
    {
        if (Status is not (ReservationStatus.Pending or ReservationStatus.Rejected or ReservationStatus.Cancelled))
        {
            throw new InvalidOperationException("Only pending, rejected or cancelled reservations can be deleted.");
        }

        SetAudit(modifiedBy);

        IsDeleted = true;
        IsActive = false;
    }

    public static int CalculateQuantity(
        DateTime deliveryDateTime,
        DateTime expectedReturnDateTime,
        RentalType rentalType)
    {
        if (expectedReturnDateTime <= deliveryDateTime)
        {
            throw new ArgumentException("Expected return date must be greater than delivery date.");
        }

        if (!Enum.IsDefined(rentalType))
        {
            throw new ArgumentException("Rental type is invalid.");
        }

        double days = (expectedReturnDateTime.Date - deliveryDateTime.Date).TotalDays;

        return Math.Max(
            1,
            rentalType switch
            {
                RentalType.Daily => (int)Math.Ceiling(days),
                RentalType.Weekly => (int)Math.Ceiling(days / 7),
                RentalType.Monthly => (int)Math.Ceiling(days / 30),
                _ => throw new ArgumentException("Rental type is invalid.")
            });
    }

    private static void Validate(
        Guid tenantId,
        string code,
        Guid customerId,
        Guid vehicleId,
        DateTime delivery,
        DateTime expectedReturn,
        RentalType rentalType,
        int quantity,
        decimal unitRate,
        bool depositRequired,
        decimal deposit,
        string deliveryName,
        string? deliveryAddress,
        decimal deliveryFee,
        string returnName,
        string? returnAddress,
        decimal returnFee,
        decimal discount,
        ReservationChannel channel,
        string? notes,
        string actor)
    {
        if (tenantId == Guid.Empty || customerId == Guid.Empty || vehicleId == Guid.Empty)
        {
            throw new ArgumentException("Tenant, customer and vehicle identifiers are required.");
        }

        if (string.IsNullOrWhiteSpace(code) || code.Trim().Length > 30)
        {
            throw new ArgumentException("Reservation code is required and cannot exceed 30 characters.");
        }

        if (expectedReturn <= delivery)
        {
            throw new ArgumentException("Expected return date must be greater than delivery date.");
        }

        if (!Enum.IsDefined(rentalType) || !Enum.IsDefined(channel))
        {
            throw new ArgumentException("Reservation type or channel is invalid.");
        }

        if (quantity <= 0
            || unitRate <= 0
            || deposit < 0
            || deliveryFee < 0
            || returnFee < 0
            || discount < 0)
        {
            throw new ArgumentException("Reservation amounts are invalid.");
        }

        if (depositRequired && deposit <= 0)
        {
            throw new ArgumentException("Security deposit amount must be greater than zero when required.");
        }

        decimal rentalAmount = unitRate * quantity;

        if (discount > rentalAmount)
        {
            throw new ArgumentException("Discount amount cannot exceed rental amount.");
        }

        ValidateText(deliveryName, 150, "Delivery location name");
        ValidateOptional(deliveryAddress, 500, "Delivery address details");
        ValidateText(returnName, 150, "Return location name");
        ValidateOptional(returnAddress, 500, "Return address details");
        ValidateOptional(notes, 1000, "Notes");
        SetActor(actor, out _);

        decimal totalAmount = CalculateTotal(
            rentalAmount,
            depositRequired ? deposit : 0,
            deliveryFee,
            returnFee,
            discount);

        if (totalAmount < 0)
        {
            throw new ArgumentException("Total amount cannot be negative.");
        }
    }

    private static decimal CalculateTotal(
        decimal rental,
        decimal deposit,
        decimal deliveryFee,
        decimal returnFee,
        decimal discount)
    {
        return rental + deposit + deliveryFee + returnFee - discount;
    }

    private void EnsurePending()
    {
        if (Status != ReservationStatus.Pending)
        {
            throw new InvalidOperationException("Only pending reservations can be updated.");
        }
    }

    private void SetAudit(string actor)
    {
        SetActor(actor, out string normalized);

        ModifiedBy = normalized;
        ModifiedDate = DateTime.UtcNow;
    }

    private static void SetActor(
        string actor,
        out string normalized)
    {
        if (string.IsNullOrWhiteSpace(actor))
        {
            throw new ArgumentException("Actor is required.");
        }

        normalized = actor.Trim();
    }

    private static void ValidateReason(string reason)
    {
        ValidateText(reason, 500, "Reason");
    }

    private static void ValidateText(
        string value,
        int max,
        string name)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Trim().Length > max)
        {
            throw new ArgumentException($"{name} is required and cannot exceed {max} characters.");
        }
    }

    private static void ValidateOptional(
        string? value,
        int max,
        string name)
    {
        if (Normalize(value)?.Length > max)
        {
            throw new ArgumentException($"{name} cannot exceed {max} characters.");
        }
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
