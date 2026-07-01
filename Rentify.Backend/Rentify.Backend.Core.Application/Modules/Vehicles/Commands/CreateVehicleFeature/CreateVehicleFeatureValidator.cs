using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.CreateVehicleFeature;

public sealed class CreateVehicleFeatureValidator : AbstractValidator<CreateVehicleFeatureCommand>
{
    public CreateVehicleFeatureValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
        RuleFor(x => x.CreatedBy).NotEmpty();
    }
}
