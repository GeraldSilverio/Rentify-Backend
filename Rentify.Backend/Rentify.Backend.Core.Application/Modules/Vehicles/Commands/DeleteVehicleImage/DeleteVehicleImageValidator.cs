using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.DeleteVehicleImage;

public sealed class DeleteVehicleImageValidator : AbstractValidator<DeleteVehicleImageCommand>
{
    public DeleteVehicleImageValidator()
    {
        RuleFor(x => x.VehicleId).NotEmpty().WithMessage("Vehicle Id is required.");
        RuleFor(x => x.ImageId).NotEmpty().WithMessage("Image Id is required.");
        RuleFor(x => x.ModifiedBy).NotEmpty().WithMessage("Modified by is required.");
    }
}
