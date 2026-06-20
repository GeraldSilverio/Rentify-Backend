using Rentify.Backend.Core.Domain.Commons;
using Rentify.Backend.Core.Domain.Entities.Reservations;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Domain.Entities.Payments;

public sealed class Payment : BaseEntity
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid ReservationId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentMethod Method { get; private set; }
    public string Reference { get; private set; } = null!;
    public DateTime PaidAt { get; private set; }
    public Reservation Reservation { get; private set; } = null!;
    public Invoice Invoice { get; private set; } = null!;

    private Payment()
    {
    }

    private Payment(
        Guid tenantId,
        Guid reservationId,
        decimal amount,
        PaymentMethod method,
        string reference,
        string createdBy)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        ReservationId = reservationId;
        Amount = amount;
        Method = method;
        Reference = reference.Trim();
        PaidAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        ModifiedBy = createdBy;
        CreatedDate = PaidAt;
        ModifiedDate = CreatedDate;
        IsActive = true;
    }

    public static Payment Create(
        Guid tenantId,
        Guid reservationId,
        decimal amount,
        PaymentMethod method,
        string reference,
        string createdBy)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant Id is required.");

        if (reservationId == Guid.Empty)
            throw new ArgumentException("Reservation Id is required.");

        if (amount <= 0)
            throw new ArgumentException("Payment amount must be greater than zero.");

        return new Payment(tenantId, reservationId, amount, method, reference, createdBy);
    }
}
