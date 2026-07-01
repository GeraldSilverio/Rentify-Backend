using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.ReplaceVehicleFeatures;

public sealed class ReplaceVehicleFeaturesValidator : AbstractValidator<ReplaceVehicleFeaturesCommand>
{
    public ReplaceVehicleFeaturesValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.VehicleId).NotEmpty();
        RuleFor(x => x.FeatureIds).NotNull();
        RuleFor(x => x.FeatureIds)
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("Vehicle features cannot contain duplicated values.")
            .When(x => x.FeatureIds is not null);
        RuleFor(x => x.ModifiedBy).NotEmpty();
    }
}
