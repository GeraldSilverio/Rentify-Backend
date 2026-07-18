using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Locations.Commands.CreateLocation;

public sealed class CreateLocationValidator : AbstractValidator<CreateLocationCommand>
{
    public CreateLocationValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.City).MaximumLength(100);
        RuleFor(x => x.Province).MaximumLength(100);
        RuleFor(x => x.Country).MaximumLength(100);
        RuleFor(x => x.Address).MaximumLength(250);
        RuleFor(x => x.Notes).MaximumLength(500);
        RuleFor(x => x.CreatedBy).NotEmpty();
    }
}
