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

        RuleFor(x => x.OwnerFullName)
            .NotEmpty().WithMessage("OwnerFullName is required")
            .MaximumLength(150);

        RuleFor(x => x.OwnerUserName)
            .NotEmpty().WithMessage("OwnerUserName is required")
            .MaximumLength(100);

        RuleFor(x => x.OwnerEmail)
            .NotEmpty().WithMessage("OwnerEmail is required")
            .EmailAddress().WithMessage("Please enter a valid owner email address.");

        RuleFor(x => x.OwnerPhoneNumber)
            .NotEmpty().WithMessage("OwnerPhoneNumber is required")
            .MaximumLength(30);

        RuleFor(x => x.OwnerPassword)
            .NotEmpty().WithMessage("OwnerPassword is required")
            .MinimumLength(8).WithMessage("OwnerPassword must have 8 characters")
            .Matches("[A-Z]").WithMessage("OwnerPassword must have 1 capital letter")
            .Matches("[a-z]").WithMessage("OwnerPassword must have 1 minuscule letter")
            .Matches("[0-9]").WithMessage("OwnerPassword must have 1 number")
            .Matches("[^a-zA-Z0-9]").WithMessage("OwnerPassword must have 1 special character");

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
