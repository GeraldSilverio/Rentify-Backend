using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.RegisterTenant;

public sealed class RegisterTenantValidator
    : AbstractValidator<RegisterTenantCommand>
{
    public RegisterTenantValidator()
    {
        RuleFor(x => x.LegalName).NotEmpty()
            .NotNull().WithMessage("Legalname is required")
            .MaximumLength(150).WithMessage("Legalname ");

        RuleFor(x => x.Name).NotEmpty()
            .NotNull().WithMessage("Name is required");

        RuleFor(x => x.Rnc).NotEmpty()
            .NotNull().WithMessage("Rnc is required");

        RuleFor(x => x.UserInformation.FullName).NotNull()
            .NotEmpty().WithMessage("OwnerFullName is required")
            .MaximumLength(150);

        RuleFor(x => x.UserInformation.UserName)
            .NotEmpty().WithMessage("OwnerUserName is required")
            .MaximumLength(100);

        RuleFor(x => x.UserInformation.ContactInformation.Email)
            .NotEmpty().WithMessage("OwnerEmail is required")
            .EmailAddress().WithMessage("Please enter a valid owner email address.");

        RuleFor(x => x.UserInformation.ContactInformation.PhoneNumber)
            .NotEmpty().WithMessage("OwnerPhoneNumber is required")
            .MaximumLength(30);

        RuleFor(x => x.UserInformation.Password)
            .NotEmpty().WithMessage("OwnerPassword is required")
            .MinimumLength(8).WithMessage("La contraseña debe tener minimo 8 caracteres")
            .Matches("[A-Z]").WithMessage("La contraseña debe tener minimo 1 caracter en mayuscula.")
            .Matches("[a-z]").WithMessage("La contraseña debe tener minimo 1 caracter en minuscula.")
            .Matches("[0-9]").WithMessage("La contraseña debe tener 1 número.")
            .Matches("[^a-zA-Z0-9]").WithMessage("La contraseña debe tener 1 caracter especial.");

        RuleFor(x => x.CreatedBy)
            .NotEmpty().NotNull().WithMessage("CreatedBy is required")
            .MaximumLength(100);

        RuleFor(x => x.SubscriptionPlanCode)
            .NotEmpty().NotNull().WithMessage("SubscriptionPlanCode is required")
            .MaximumLength(50);

        RuleFor(x => x.TrialDays)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(90);

        RuleFor(x => x.BusinessModel).NotNull().NotEmpty().WithMessage("BusinessModel is required");
    }
}
