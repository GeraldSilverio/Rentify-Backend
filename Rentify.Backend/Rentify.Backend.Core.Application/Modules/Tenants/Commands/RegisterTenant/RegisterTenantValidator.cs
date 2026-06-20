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
            .MinimumLength(8).WithMessage("La contraseña debe tener minimo 8 caracteres")
            .Matches("[A-Z]").WithMessage("La contraseña debe tener minimo 1 caracter en mayuscula.")
            .Matches("[a-z]").WithMessage("La contraseña debe tener minimo 1 caracter en minuscula.")
            .Matches("[0-9]").WithMessage("La contraseña debe tener 1 número.")
            .Matches("[^a-zA-Z0-9]").WithMessage("La contraseña debe tener 1 caracter especial.");

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
