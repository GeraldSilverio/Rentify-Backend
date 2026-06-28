using FluentValidation;
using System.Text.RegularExpressions;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.UpdateAdminTenant;

public sealed class UpdateAdminTenantValidator : AbstractValidator<UpdateAdminTenantCommand>
{
    public UpdateAdminTenantValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.Name)
            .NotNull()
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.LegalName)
            .MaximumLength(150)
            .When(x => x.LegalName is not null);

        RuleFor(x => x.Rnc)
            .Must(rnc => rnc is null || !string.IsNullOrWhiteSpace(rnc))
            .WithMessage("RNC cannot be empty.")
            .Must(rnc => rnc is null || IsValidNormalizedRnc(NormalizeRnc(rnc)))
            .WithMessage("RNC must contain 9 or 11 digits.")
            .When(x => x.Rnc is not null);

        RuleFor(x => x.BusinessModel)
            .NotNull()
            .IsInEnum();

        RuleFor(x => x.ModifiedBy).NotEmpty();
    }

    private static string NormalizeRnc(string rnc)
    {
        return rnc.Trim().Replace("-", string.Empty).Replace(" ", string.Empty);
    }

    private static bool IsValidNormalizedRnc(string rnc)
    {
        return (rnc.Length == 9 || rnc.Length == 11) && Regex.IsMatch(rnc, "^[0-9]+$");
    }
}
