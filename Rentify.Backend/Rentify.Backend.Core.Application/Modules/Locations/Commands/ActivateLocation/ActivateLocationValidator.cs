using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Locations.Commands.ActivateLocation;

public sealed class ActivateLocationValidator : AbstractValidator<ActivateLocationCommand>
{
    public ActivateLocationValidator()
    {
        RuleFor(x => x.LocationId).NotEmpty();
        RuleFor(x => x.ModifiedBy).NotEmpty();
    }
}
