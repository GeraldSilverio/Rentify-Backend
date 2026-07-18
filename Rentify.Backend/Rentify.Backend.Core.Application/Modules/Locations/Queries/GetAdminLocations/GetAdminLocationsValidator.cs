using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Locations.Queries.GetAdminLocations;

public sealed class GetAdminLocationsValidator : AbstractValidator<GetAdminLocationsQuery>
{
    public GetAdminLocationsValidator()
    {
        RuleFor(x => x.Type).IsInEnum().When(x => x.Type.HasValue);
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
