using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Locations.Queries.GetTenantLocationById;

public sealed class GetTenantLocationByIdValidator : AbstractValidator<GetTenantLocationByIdQuery>
{
    public GetTenantLocationByIdValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.TenantLocationId).NotEmpty();
    }
}
