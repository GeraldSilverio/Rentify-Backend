using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Locations.Commands.DeactivateTenantLocation;

public sealed class DeactivateTenantLocationValidator : AbstractValidator<DeactivateTenantLocationCommand>
{
    public DeactivateTenantLocationValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.TenantLocationId).NotEmpty();
        RuleFor(x => x.ModifiedBy).NotEmpty();
    }
}
