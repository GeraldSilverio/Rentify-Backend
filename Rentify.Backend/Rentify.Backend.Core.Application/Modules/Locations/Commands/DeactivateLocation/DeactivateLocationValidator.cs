using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Locations.Commands.DeactivateLocation;

public sealed class DeactivateLocationValidator : AbstractValidator<DeactivateLocationCommand>
{
    public DeactivateLocationValidator()
    {
        RuleFor(x => x.LocationId).NotEmpty();
        RuleFor(x => x.ModifiedBy).NotEmpty();
    }
}
