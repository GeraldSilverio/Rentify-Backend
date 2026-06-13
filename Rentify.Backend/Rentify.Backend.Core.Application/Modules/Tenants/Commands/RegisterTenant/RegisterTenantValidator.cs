using FluentValidation;
using Rentify.Backend.Core.Application.Modules.Tenants.Commands.RegisterTenant;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.RegisterTenant;

public sealed class RegisterTenantValidator
    : AbstractValidator<RegisterTenantCommand>
{
    public RegisterTenantValidator()
    {
        RuleFor(x => x.RentCarName)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.CreatedBy)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.SubscriptionPlanCode)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.TrialDays)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(90);
    }
}
