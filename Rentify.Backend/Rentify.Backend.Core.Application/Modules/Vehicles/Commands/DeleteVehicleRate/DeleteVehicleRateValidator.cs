using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.DeleteVehicleRate;

public sealed class DeleteVehicleRateValidator : AbstractValidator<DeleteVehicleRateCommand>
{
    public DeleteVehicleRateValidator()
    {
        RuleFor(command => command.TenantId).NotEmpty();
        RuleFor(command => command.VehicleId).NotEmpty();
        RuleFor(command => command.RateId).NotEmpty();
        RuleFor(command => command.ModifiedBy).NotEmpty();
    }
}
