using Rentify.Backend.Core.Domain.Commons;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Domain.Entities.Reservations;

public sealed class ReservationPayment : BaseEntity
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid ReservationId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentMethod Method { get; private set; }
    public string Reference { get; private set; } = null!;
    public Reservation Reservation { get; private set; } = null!;

    private ReservationPayment()
    {
    }

    private ReservationPayment(
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
        CreatedBy = createdBy;
        ModifiedBy = createdBy;
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = CreatedDate;
        IsActive = true;
    }

    public static ReservationPayment Create(
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

        return new ReservationPayment(tenantId, reservationId, amount, method, reference, createdBy);
    }
}
