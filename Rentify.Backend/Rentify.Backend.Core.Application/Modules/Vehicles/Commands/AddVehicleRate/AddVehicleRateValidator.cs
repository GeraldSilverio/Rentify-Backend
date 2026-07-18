using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.AddVehicleRate;

public sealed class AddVehicleRateValidator : AbstractValidator<AddVehicleRateCommand>
{
    public AddVehicleRateValidator()
    {
        RuleFor(command => command.TenantId).NotEmpty();
        RuleFor(command => command.VehicleId).NotEmpty();
        RuleFor(command => command.RentalType).IsInEnum().WithMessage("El tipo de renta no es válido.");
        RuleFor(command => command.Price).GreaterThan(0).WithMessage("El precio de la tarifa debe ser mayor que cero.");
        RuleFor(command => command.CreatedBy).NotEmpty();
    }
}
