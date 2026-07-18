using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Locations.Commands.UpdateTenantLocation;

public sealed class UpdateTenantLocationValidator : AbstractValidator<UpdateTenantLocationCommand>
{
    public UpdateTenantLocationValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.TenantLocationId).NotEmpty();
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(150);
        RuleFor(x => x).Must(x => x.AllowsDelivery || x.AllowsPickup).WithMessage("Debe permitir entrega o recogida.");
        RuleFor(x => x.DeliveryFee).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PickupFee).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ModifiedBy).NotEmpty();
    }
}
