using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.ManageVehicleCatalog;

public sealed class ManageVehicleCatalogValidator : AbstractValidator<ManageVehicleCatalogCommand>
{
    public ManageVehicleCatalogValidator()
    {
        RuleFor(x => x.CatalogKind).IsInEnum();
        RuleFor(x => x.Action).IsInEnum();
        RuleFor(x => x.ModifiedBy).NotEmpty();

        RuleFor(x => x.CatalogId)
            .NotNull()
            .NotEqual(Guid.Empty)
            .When(x => x.Action is not VehicleCatalogAction.Create);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .When(x => x.Action is VehicleCatalogAction.Create or VehicleCatalogAction.Update);

        RuleFor(x => x.VehicleBrandId)
            .NotNull()
            .NotEqual(Guid.Empty)
            .When(x => x.CatalogKind == VehicleCatalogKind.Model &&
                       x.Action is VehicleCatalogAction.Create or VehicleCatalogAction.Update);
    }
}
