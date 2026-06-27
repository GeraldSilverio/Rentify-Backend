using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.DeactivateAdminTenant;

public sealed class DeactivateAdminTenantValidator : AbstractValidator<DeactivateAdminTenantCommand>
{
    public DeactivateAdminTenantValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.ModifiedBy).NotEmpty();
    }
}
