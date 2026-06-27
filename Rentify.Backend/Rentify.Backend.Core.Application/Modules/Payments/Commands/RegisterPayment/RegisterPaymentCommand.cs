using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Payments.Commands.RegisterPayment;

public sealed record RegisterPaymentCommand(
    Guid TenantId,
    Guid ReservationId,
    decimal Amount,
    PaymentMethod Method,
    string Reference,
    string CreatedBy) : IRequest<ResultReponse<RegisterPaymentResponse>>;
