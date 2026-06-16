using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.DeleteVehicle;

public sealed class DeleteVehicleValidator : AbstractValidator<DeleteVehicleCommand>
{
    public DeleteVehicleValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.VehicleId).NotEmpty();
        RuleFor(x => x.ModifiedBy).NotEmpty();
    }
}
