using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Locations.Queries.GetAvailableLocations;

public sealed class GetAvailableLocationsValidator : AbstractValidator<GetAvailableLocationsQuery>
{
    public GetAvailableLocationsValidator()
    {
        RuleFor(x => x.Type).IsInEnum().When(x => x.Type.HasValue);
    }
}
