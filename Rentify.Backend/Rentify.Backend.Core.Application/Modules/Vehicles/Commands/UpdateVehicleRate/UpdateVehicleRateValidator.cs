using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UpdateVehicleRate;

public sealed class UpdateVehicleRateValidator : AbstractValidator<UpdateVehicleRateCommand>
{
    public UpdateVehicleRateValidator()
    {
        RuleFor(command => command.TenantId).NotEmpty();
        RuleFor(command => command.VehicleId).NotEmpty();
        RuleFor(command => command.RateId).NotEmpty();
        RuleFor(command => command.Price).GreaterThan(0).WithMessage("El precio de la tarifa debe ser mayor que cero.");
        RuleFor(command => command.ModifiedBy).NotEmpty();
    }
}
