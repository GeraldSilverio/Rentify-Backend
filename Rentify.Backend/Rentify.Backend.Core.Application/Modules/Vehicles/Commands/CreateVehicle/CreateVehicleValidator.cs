using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.CreateVehicle;

public sealed class CreateVehicleValidator : AbstractValidator<CreateVehicleCommand>
{
    public CreateVehicleValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty().WithMessage("Tenant Id is required.");
        RuleFor(x => x.RentCarId).NotEmpty().WithMessage("Rent car Id is required.");
        RuleFor(x => x.VehicleModelId).NotEmpty().WithMessage("Vehicle model Id is required.");
        RuleFor(x => x.VehicleTypeId).NotEmpty().WithMessage("Vehicle type Id is required.");
        RuleFor(x => x.Year)
            .InclusiveBetween(1980, DateTime.UtcNow.Year + 1)
            .WithMessage("Vehicle year is invalid.");
        RuleFor(x => x.PlateNumber).NotEmpty().WithMessage("Plate number is required.").MaximumLength(20);
        RuleFor(x => x.Vin)
            .NotEmpty()
            .WithMessage("VIN is required.")
            .Length(17)
            .WithMessage("VIN must contain 17 characters.")
            .Matches("^[A-HJ-NPR-Z0-9]{17}$")
            .WithMessage("VIN contains invalid characters.");
        RuleFor(x => x.Color).NotEmpty().WithMessage("Color is required.").MaximumLength(50);
        RuleFor(x => x.DailyRate).GreaterThan(0).WithMessage("Daily rate must be greater than zero.");
        RuleFor(x => x.CreatedBy).NotEmpty().WithMessage("Created by is required.");
    }
}
