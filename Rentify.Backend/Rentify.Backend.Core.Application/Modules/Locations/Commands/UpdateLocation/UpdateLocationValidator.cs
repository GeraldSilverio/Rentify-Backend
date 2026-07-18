using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Locations.Commands.UpdateLocation;

public sealed class UpdateLocationValidator : AbstractValidator<UpdateLocationCommand>
{
    public UpdateLocationValidator()
    {
        RuleFor(x => x.LocationId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.City).MaximumLength(100);
        RuleFor(x => x.Province).MaximumLength(100);
        RuleFor(x => x.Country).MaximumLength(100);
        RuleFor(x => x.Address).MaximumLength(250);
        RuleFor(x => x.Notes).MaximumLength(500);
        RuleFor(x => x.ModifiedBy).NotEmpty();
    }
}
