using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Locations.Commands.ActivateTenantLocation;

public sealed class ActivateTenantLocationValidator : AbstractValidator<ActivateTenantLocationCommand>
{
    public ActivateTenantLocationValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.TenantLocationId).NotEmpty();
        RuleFor(x => x.ModifiedBy).NotEmpty();
    }
}
