using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Locations.Commands.DeleteLocation;

public sealed class DeleteLocationValidator : AbstractValidator<DeleteLocationCommand>
{
    public DeleteLocationValidator()
    {
        RuleFor(x => x.LocationId).NotEmpty();
        RuleFor(x => x.ModifiedBy).NotEmpty();
    }
}
