using Rentify.Backend.Core.Domain.Commons;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Domain.Entities;

public sealed class Reservation : BaseEntity
{
    private readonly List<ReservationVehicle> _vehicles = [];
    private readonly List<ReservationPayment> _payments = [];

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid CustomerId { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public int RentalDays { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal PaidAmount { get; private set; }
    public ReservationStatus Status { get; private set; }
    public PaymentStatus PaymentStatus { get; private set; }
    public Customer Customer { get; private set; } = null!;
    public IReadOnlyCollection<ReservationVehicle> Vehicles => _vehicles.AsReadOnly();
    public IReadOnlyCollection<ReservationPayment> Payments => _payments.AsReadOnly();

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
        StartDate = startDate;
        EndDate = endDate;
        RentalDays = CalculateRentalDays(startDate, endDate);
        Status = ReservationStatus.Confirmed;
        PaymentStatus = PaymentStatus.Pending;
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

    public void AddVehicle(Guid vehicleId, decimal dailyRate, string createdBy)
    {
        if (_vehicles.Any(x => x.VehicleId == vehicleId && !x.IsDeleted))
            throw new ArgumentException("Vehicle is already linked to this reservation.");

        ReservationVehicle reservationVehicle = ReservationVehicle.Create(
            TenantId,
            Id,
            vehicleId,
            dailyRate,
            RentalDays,
            createdBy);

        _vehicles.Add(reservationVehicle);
        RecalculateTotal();
        ModifiedBy = createdBy;
        ModifiedDate = DateTime.UtcNow;
    }

    public ReservationPayment RegisterPayment(
        decimal amount,
        PaymentMethod method,
        string reference,
        string createdBy)
    {
        if (amount <= 0)
            throw new ArgumentException("Payment amount must be greater than zero.");

        if (PaidAmount + amount > TotalAmount)
            throw new ArgumentException("Payment amount exceeds reservation balance.");

        ReservationPayment payment = ReservationPayment.Create(TenantId, Id, amount, method, reference, createdBy);
        _payments.Add(payment);
        PaidAmount += amount;
        PaymentStatus = PaidAmount >= TotalAmount ? PaymentStatus.Paid : PaymentStatus.Partial;
        ModifiedBy = createdBy;
        ModifiedDate = DateTime.UtcNow;

        return payment;
    }

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

    private void RecalculateTotal()
    {
        TotalAmount = _vehicles.Where(x => !x.IsDeleted).Sum(x => x.TotalAmount);
    }
}
