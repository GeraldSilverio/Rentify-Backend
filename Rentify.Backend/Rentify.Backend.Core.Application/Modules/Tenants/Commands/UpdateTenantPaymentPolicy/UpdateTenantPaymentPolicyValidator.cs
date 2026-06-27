using FluentValidation;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.UpdateTenantPaymentPolicy;

public sealed class UpdateTenantPaymentPolicyValidator : AbstractValidator<UpdateTenantPaymentPolicyCommand>
{
    public UpdateTenantPaymentPolicyValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PaymentFrequency).IsInEnum();
        RuleFor(x => x.CutoffDayOfWeek).IsInEnum();
        RuleFor(x => x.ReminderStartDayOfWeek).IsInEnum();
        RuleFor(x => x.GraceDays).InclusiveBetween(0, 15);
        RuleFor(x => x.ModifiedBy).NotEmpty();

        When(x => x.PaymentFrequency == PaymentFrequency.Weekly, () =>
        {
            RuleFor(x => x.CutoffDayOfWeek).IsInEnum();
        });
    }
}
