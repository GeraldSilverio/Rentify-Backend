using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Locations.Queries.GetTenantLocations;

public sealed class GetTenantLocationsValidator : AbstractValidator<GetTenantLocationsQuery>
{
    public GetTenantLocationsValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
