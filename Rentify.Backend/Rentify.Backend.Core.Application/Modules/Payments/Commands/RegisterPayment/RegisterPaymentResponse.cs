namespace Rentify.Backend.Core.Application.Modules.Payments.Commands.RegisterPayment;

public sealed record RegisterPaymentResponse(
    Guid PaymentId,
    Guid InvoiceId,
    string InvoiceNumber);
