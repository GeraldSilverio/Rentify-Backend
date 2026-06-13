using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.CreateVehicle;

public sealed class CreateVehicleValidator : AbstractValidator<CreateVehicleCommand>
{
    public CreateVehicleValidator()
    {
        RuleFor(x => x.RentCarId).NotEmpty().WithMessage("Rent car Id is required.");
        RuleFor(x => x.Make).NotEmpty().WithMessage("Make is required.").MaximumLength(100);
        RuleFor(x => x.Model).NotEmpty().WithMessage("Model is required.").MaximumLength(100);
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
        RuleFor(x => x.Color).MaximumLength(50);
        RuleFor(x => x.DailyRate).GreaterThan(0).WithMessage("Daily rate must be greater than zero.");
        RuleFor(x => x.CreatedBy).NotEmpty().WithMessage("Created by is required.");
    }
}
