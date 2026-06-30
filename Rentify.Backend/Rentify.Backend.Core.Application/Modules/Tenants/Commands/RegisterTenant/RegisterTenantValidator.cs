using FluentValidation;
using Rentify.Backend.Core.Application.Modules.Tenants.Validation;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.RegisterTenant;

public sealed class RegisterTenantValidator : AbstractValidator<RegisterTenantCommand>
{
    public RegisterTenantValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(150);

        RuleFor(x => x.LegalName)
            .MaximumLength(150)
            .When(x => x.LegalName is not null);

        RuleFor(x => x.Rnc)
            .Must(rnc => rnc is null || !string.IsNullOrWhiteSpace(rnc))
            .WithMessage("RNC cannot be empty.")
            .Must(TenantRncRules.IsValidDominicanRnc)
            .WithMessage("RNC must contain 9 or 11 digits.")
            .When(x => x.Rnc is not null);

        RuleFor(x => x.BusinessModel)
            .IsInEnum()
            .WithMessage("BusinessModel must be a valid value.");

        RuleFor(x => x.UserInformation)
            .NotNull()
            .WithMessage("Owner information is required");

        When(x => x.UserInformation is not null, () =>
        {
            RuleFor(x => x.UserInformation.FullName)
                .NotEmpty().WithMessage("OwnerFullName is required")
                .MaximumLength(150);

            RuleFor(x => x.UserInformation.UserName)
                .NotEmpty().WithMessage("OwnerUserName is required")
                .MaximumLength(100);

            RuleFor(x => x.UserInformation.ContactInformation)
                .NotNull()
                .WithMessage("Owner contact information is required");

            When(x => x.UserInformation.ContactInformation is not null, () =>
            {
                RuleFor(x => x.UserInformation.ContactInformation.Email)
                    .NotEmpty().WithMessage("OwnerEmail is required")
                    .EmailAddress().WithMessage("Please enter a valid owner email address.");

                RuleFor(x => x.UserInformation.ContactInformation.PhoneNumber)
                    .NotEmpty().WithMessage("OwnerPhoneNumber is required")
                    .MaximumLength(30);
            });

            RuleFor(x => x.UserInformation.Password)
                .NotEmpty().WithMessage("OwnerPassword is required")
                .MinimumLength(8).WithMessage("La contraseña debe tener minimo 8 caracteres")
                .Matches("[A-Z]").WithMessage("La contraseña debe tener minimo 1 caracter en mayuscula.")
                .Matches("[a-z]").WithMessage("La contraseña debe tener minimo 1 caracter en minuscula.")
                .Matches("[0-9]").WithMessage("La contraseña debe tener 1 número.")
                .Matches("[^a-zA-Z0-9]").WithMessage("La contraseña debe tener 1 caracter especial.");
        });

        RuleFor(x => x.ContactInformation)
            .NotNull()
            .WithMessage("Contact information is required");

        When(x => x.ContactInformation is not null, () =>
        {
            RuleFor(x => x.ContactInformation.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Please enter a valid email address.");

            RuleFor(x => x.ContactInformation.PhoneNumber)
                .NotEmpty().WithMessage("PhoneNumber is required")
                .MaximumLength(30);
        });

        RuleFor(x => x.CreatedBy)
            .NotEmpty().WithMessage("CreatedBy is required")
            .MaximumLength(100);

        RuleFor(x => x.SubscriptionPlanCode)
            .NotEmpty().WithMessage("SubscriptionPlanCode is required")
            .MaximumLength(50);

        RuleFor(x => x.TrialDays)
            .InclusiveBetween(0, 90);
    }
}
