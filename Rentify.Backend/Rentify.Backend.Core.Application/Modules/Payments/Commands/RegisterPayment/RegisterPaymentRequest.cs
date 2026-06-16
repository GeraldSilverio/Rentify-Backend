using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Payments.Commands.RegisterPayment;

public sealed record RegisterPaymentRequest(
    Guid ReservationId,
    decimal Amount,
    PaymentMethod Method,
    string Reference,
    string CreatedBy);
