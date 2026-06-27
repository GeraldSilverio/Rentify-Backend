using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.ActivateAdminTenant;

public sealed class ActivateAdminTenantValidator : AbstractValidator<ActivateAdminTenantCommand>
{
    public ActivateAdminTenantValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.ModifiedBy).NotEmpty();
    }
}
