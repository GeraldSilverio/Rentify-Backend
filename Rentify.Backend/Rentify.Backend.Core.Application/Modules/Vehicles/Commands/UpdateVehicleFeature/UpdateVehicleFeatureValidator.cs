using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UpdateVehicleFeature;

public sealed class UpdateVehicleFeatureValidator : AbstractValidator<UpdateVehicleFeatureCommand>
{
    public UpdateVehicleFeatureValidator()
    {
        RuleFor(x => x.FeatureId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ModifiedBy).NotEmpty();
    }
}
