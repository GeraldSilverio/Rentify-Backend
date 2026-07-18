using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Locations.Commands.DeleteTenantLocation;

public sealed class DeleteTenantLocationValidator : AbstractValidator<DeleteTenantLocationCommand>
{
    public DeleteTenantLocationValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.TenantLocationId).NotEmpty();
        RuleFor(x => x.ModifiedBy).NotEmpty();
    }
}
