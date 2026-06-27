using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.UpdateAdminTenant;

public sealed class UpdateAdminTenantValidator : AbstractValidator<UpdateAdminTenantCommand>
{
    public UpdateAdminTenantValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.LegalName).MaximumLength(200);
        RuleFor(x => x.Rnc).MaximumLength(50);
        RuleFor(x => x.BusinessModel).IsInEnum();
        RuleFor(x => x.ModifiedBy).NotEmpty();
    }
}
