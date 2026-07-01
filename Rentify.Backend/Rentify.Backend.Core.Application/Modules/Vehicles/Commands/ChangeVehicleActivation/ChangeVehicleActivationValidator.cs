using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.ChangeVehicleActivation;

public sealed class ChangeVehicleActivationValidator : AbstractValidator<ChangeVehicleActivationCommand>
{
    public ChangeVehicleActivationValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.VehicleId).NotEmpty();
        RuleFor(x => x.ModifiedBy).NotEmpty();
    }
}
