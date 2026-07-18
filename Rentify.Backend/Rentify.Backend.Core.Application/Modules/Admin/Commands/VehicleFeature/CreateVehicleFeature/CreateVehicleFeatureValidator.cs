using FluentValidation;
using Rentify.Backend.Core.Application.Modules.Admin.Commands.Vehicles.CreateVehicleFeature;

namespace Rentify.Backend.Core.Application.Modules.Admin.Commands.VehicleFeature.CreateVehicleFeature;

public sealed class CreateVehicleFeatureValidator : AbstractValidator<CreateVehicleFeatureCommand>
{
    public CreateVehicleFeatureValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
        RuleFor(x => x.CreatedBy).NotEmpty();
    }
}
