using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Locations.Queries.GetAdminLocationById;

public sealed class GetAdminLocationByIdValidator : AbstractValidator<GetAdminLocationByIdQuery>
{
    public GetAdminLocationByIdValidator()
    {
        RuleFor(x => x.LocationId).NotEmpty();
    }
}
