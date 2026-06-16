using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Payments.Commands.RegisterPayment;

public sealed class RegisterPaymentValidator : AbstractValidator<RegisterPaymentCommand>
{
    public RegisterPaymentValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.ReservationId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Reference).MaximumLength(100);
        RuleFor(x => x.CreatedBy).NotEmpty();
    }
}
