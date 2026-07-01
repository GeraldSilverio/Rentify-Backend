using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.ChangeVehicleFeatureStatus;

public sealed class ChangeVehicleFeatureStatusValidator : AbstractValidator<ChangeVehicleFeatureStatusCommand>
{
    public ChangeVehicleFeatureStatusValidator()
    {
        RuleFor(x => x.FeatureId).NotEmpty();
        RuleFor(x => x.ModifiedBy).NotEmpty();
    }
}
