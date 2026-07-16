using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UpdateVehicleFeatures;

public sealed class UpdateVehicleFeaturesValidator : AbstractValidator<UpdateVehicleFeaturesCommand>
{
    public UpdateVehicleFeaturesValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.VehicleId).NotEmpty();
        RuleFor(x => x.FeatureIds).NotNull();
        RuleForEach(x => x.FeatureIds)
            .NotEmpty()
            .WithMessage("Una o más características no son válidas.");
        RuleFor(x => x.ModifiedBy).NotEmpty();
    }
}
