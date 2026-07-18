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

    public decimal DiscountAmount { get; private set; }
    public decimal TotalAmount { get; private set; }

    public string DeliveryLocation { get; private set; } = null!;
    public string? ReturnLocation { get; private set; }
    public string? Notes { get; private set; }

    public ReservationStatus Status { get; private set; }
    public ReservationChannel Channel { get; private set; }

    public DateTime? ApprovedAt { get; private set; }
    public string? ApprovedBy { get; private set; }

    public DateTime? RejectedAt { get; private set; }
    public string? RejectionReason { get; private set; }

    public DateTime? CancelledAt { get; private set; }
    public string? CancellationReason { get; private set; }

    public Customer Customer { get; private set; } = null!;
    public Vehicle Vehicle { get; private set; } = null!;

    private Reservation()
    {
    }

    private Reservation(
        Guid tenantId,
        Guid customerId,
        DateOnly startDate,
        DateOnly endDate,
        string createdBy)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        CustomerId = customerId;
        //StartDate = startDate;
        //EndDate = endDate;
        //RentalDays = CalculateRentalDays(startDate, endDate);
        Status = ReservationStatus.Confirmed;
        //PaymentStatus = PaymentStatus.Pending;
        CreatedBy = createdBy;
        ModifiedBy = createdBy;
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = CreatedDate;
        IsActive = true;
    }

    public static Reservation Create(
        Guid tenantId,
        Guid customerId,
        DateOnly startDate,
        DateOnly endDate,
        string createdBy)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant Id is required.");

        if (customerId == Guid.Empty)
            throw new ArgumentException("Customer Id is required.");

        if (endDate < startDate)
            throw new ArgumentException("Reservation end date cannot be before start date.");

        return new Reservation(tenantId, customerId, startDate, endDate, createdBy);
    }

    //public void AddVehicle(Guid vehicleId, decimal dailyRate, string createdBy)
    //{
    //    if (_vehicles.Any(x => x.VehicleId == vehicleId && !x.IsDeleted))
    //        throw new ArgumentException("Vehicle is already linked to this reservation.");

    //    ReservationVehicle reservationVehicle = ReservationVehicle.Create(
    //        TenantId,
    //        Id,
    //        vehicleId,
    //        dailyRate,
    //        //RentalDays,
    //        createdBy);

    //    _vehicles.Add(reservationVehicle);
    //    RecalculateTotal();
    //    ModifiedBy = createdBy;
    //    ModifiedDate = DateTime.UtcNow;
    //}

    //public ReservationPayment RegisterPayment(
    //    decimal amount,
    //    PaymentMethod method,
    //    string reference,
    //    string createdBy)
    //{
    //    if (amount <= 0)
    //        throw new ArgumentException("Payment amount must be greater than zero.");

    //    if (PaidAmount + amount > TotalAmount)
    //        throw new ArgumentException("Payment amount exceeds reservation balance.");

    //    ReservationPayment payment = ReservationPayment.Create(TenantId, Id, amount, method, reference, createdBy);
    //    _payments.Add(payment);
    //    PaidAmount += amount;
    //    PaymentStatus = PaidAmount >= TotalAmount ? PaymentStatus.Paid : PaymentStatus.Partial;
    //    ModifiedBy = createdBy;
    //    ModifiedDate = DateTime.UtcNow;

    //    return payment;
    //}

    public void Cancel(string modifiedBy)
    {
        Status = ReservationStatus.Cancelled;
        ModifiedBy = modifiedBy;
        ModifiedDate = DateTime.UtcNow;
    }

    public static int CalculateRentalDays(DateOnly startDate, DateOnly endDate)
    {
        return Math.Max(1, endDate.DayNumber - startDate.DayNumber + 1);
    }

    //private void RecalculateTotal()
    //{
    //    TotalAmount = _vehicles.Where(x => !x.IsDeleted).Sum(x => x.TotalAmount);
    //}
}
