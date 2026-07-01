using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.DeleteVehicleImage;

public sealed class DeleteVehicleImageValidator : AbstractValidator<DeleteVehicleImageCommand>
{
    public DeleteVehicleImageValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.VehicleId).NotEmpty();
        RuleFor(x => x.ImageId).NotEmpty();
        RuleFor(x => x.ModifiedBy).NotEmpty();
    }
}
