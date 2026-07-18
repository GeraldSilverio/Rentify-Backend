using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Locations.Commands.CreateTenantLocation;

public sealed class CreateTenantLocationValidator : AbstractValidator<CreateTenantLocationCommand>
{
    public CreateTenantLocationValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.LocationId).NotEmpty().When(x => !x.IsCustom);
        RuleFor(x => x.LocationId).Null().When(x => x.IsCustom);
        RuleFor(x => x.DisplayName).NotEmpty().When(x => x.IsCustom);
        RuleFor(x => x.DisplayName).MaximumLength(150);
        RuleFor(x => x).Must(x => x.AllowsDelivery || x.AllowsPickup).WithMessage("Debe permitir entrega o recogida.");
        RuleFor(x => x.DeliveryFee).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PickupFee).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CreatedBy).NotEmpty();
    }
}
