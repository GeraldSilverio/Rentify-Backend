using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.ExtendTenantTrial;

public sealed class ExtendTenantTrialValidator : AbstractValidator<ExtendTenantTrialCommand>
{
    public ExtendTenantTrialValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.DaysToAdd).InclusiveBetween(1, 90);
        RuleFor(x => x.ModifiedBy).NotEmpty();
    }
}
