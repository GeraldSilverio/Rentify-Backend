using Rentify.Backend.Core.Domain.Commons;

namespace Rentify.Backend.Core.Domain.Entities;

public sealed class Invoice : BaseEntity
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid PaymentId { get; private set; }
    public Guid ReservationId { get; private set; }
    public string InvoiceNumber { get; private set; } = null!;
    public decimal Amount { get; private set; }
    public DateTime IssuedAt { get; private set; }
    public Payment Payment { get; private set; } = null!;

    private Invoice()
    {
    }

    private Invoice(
        Guid tenantId,
        Guid paymentId,
        Guid reservationId,
        string invoiceNumber,
        decimal amount,
        string createdBy)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        PaymentId = paymentId;
        ReservationId = reservationId;
        InvoiceNumber = invoiceNumber.Trim();
        Amount = amount;
        IssuedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        ModifiedBy = createdBy;
        CreatedDate = IssuedAt;
        ModifiedDate = CreatedDate;
        IsActive = true;
    }

    public static Invoice Create(
        Guid tenantId,
        Guid paymentId,
        Guid reservationId,
        string invoiceNumber,
        decimal amount,
        string createdBy)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant Id is required.");

        if (paymentId == Guid.Empty)
            throw new ArgumentException("Payment Id is required.");

        if (reservationId == Guid.Empty)
            throw new ArgumentException("Reservation Id is required.");

        if (string.IsNullOrWhiteSpace(invoiceNumber))
            throw new ArgumentException("Invoice number is required.");

        if (amount <= 0)
            throw new ArgumentException("Invoice amount must be greater than zero.");

        return new Invoice(tenantId, paymentId, reservationId, invoiceNumber, amount, createdBy);
    }
}
