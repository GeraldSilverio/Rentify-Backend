using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.SetPrimaryVehicleImage;

public sealed class SetPrimaryVehicleImageValidator : AbstractValidator<SetPrimaryVehicleImageCommand>
{
    public SetPrimaryVehicleImageValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.VehicleId).NotEmpty();
        RuleFor(x => x.ImageId).NotEmpty();
        RuleFor(x => x.ModifiedBy).NotEmpty();
    }
}
