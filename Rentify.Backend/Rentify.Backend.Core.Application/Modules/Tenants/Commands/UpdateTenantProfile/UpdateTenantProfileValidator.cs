using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.UpdateTenantProfile;

public sealed class UpdateTenantProfileValidator : AbstractValidator<UpdateTenantProfileCommand>
{
    public UpdateTenantProfileValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.LegalName).MaximumLength(200);
        RuleFor(x => x.Rnc).MaximumLength(50);
        RuleFor(x => x.ModifiedBy).NotEmpty();
    }
}
