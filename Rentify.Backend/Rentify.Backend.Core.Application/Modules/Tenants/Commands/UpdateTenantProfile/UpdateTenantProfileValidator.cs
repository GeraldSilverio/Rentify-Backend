using FluentValidation;
using Rentify.Backend.Core.Application.Modules.Tenants.Validation;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.UpdateTenantProfile;

public sealed class UpdateTenantProfileValidator : AbstractValidator<UpdateTenantProfileCommand>
{
    public UpdateTenantProfileValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.Name)
            .NotEmpty()
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

        RuleFor(x => x.ModifiedBy).NotEmpty();
    }
}
