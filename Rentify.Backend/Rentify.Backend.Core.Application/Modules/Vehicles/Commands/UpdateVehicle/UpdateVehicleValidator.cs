using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UpdateVehicle;

public sealed class UpdateVehicleValidator : AbstractValidator<UpdateVehicleCommand>
{
    public UpdateVehicleValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty().WithMessage("Tenant Id is required.");
        RuleFor(x => x.VehicleId).NotEmpty().WithMessage("Vehicle Id is required.");
        RuleFor(x => x.VehicleBrandId).NotEmpty().WithMessage("Vehicle brand Id is required.");
        RuleFor(x => x.VehicleModelId).NotEmpty().WithMessage("Vehicle model Id is required.");
        RuleFor(x => x.VehicleTypeId).NotEmpty().WithMessage("Vehicle type Id is required.");
        RuleFor(x => x.Year)
            .InclusiveBetween(1980, DateTime.UtcNow.Year + 1)
            .WithMessage("Vehicle year is invalid.");
        RuleFor(x => x.PlateNumber).NotEmpty().WithMessage("Plate number is required.").MaximumLength(20);
        RuleFor(x => x.Vin)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.Vin));
        RuleFor(x => x.Color).NotEmpty().MaximumLength(50);
        RuleFor(x => x.DailyRate).GreaterThan(0);
        RuleFor(x => x.CurrentMileage)
            .GreaterThanOrEqualTo(0)
            .When(x => x.CurrentMileage.HasValue);
        RuleFor(x => x.ModifiedBy).NotEmpty().WithMessage("Modified by is required.");
    }
}
