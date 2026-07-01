using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.CreateVehicle;

public sealed class CreateVehicleValidator : AbstractValidator<CreateVehicleCommand>
{
    public CreateVehicleValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty().WithMessage("Tenant Id is required.");
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
        RuleFor(x => x.Color).NotEmpty().WithMessage("Color is required.").MaximumLength(50);
        RuleFor(x => x.CurrentMileage)
            .GreaterThanOrEqualTo(0)
            .When(x => x.CurrentMileage.HasValue);
        RuleFor(x => x.Rates)
            .NotEmpty()
            .WithMessage("At least one vehicle rate is required.");
        RuleForEach(x => x.Rates)
            .SetValidator(new CreateVehicleRateValidator());
        RuleFor(x => x.Rates)
            .Must(rates => rates.Select(x => x.RentalType).Distinct().Count() == rates.Count)
            .WithMessage("Vehicle rates cannot contain duplicated rental types.")
            .When(x => x.Rates.Count > 0);
        RuleFor(x => x.FeatureIds)
            .Must(featureIds => featureIds.All(id => id != Guid.Empty))
            .WithMessage("Vehicle features cannot contain empty identifiers.");
        RuleFor(x => x.FeatureIds)
            .Must(featureIds => featureIds.Distinct().Count() == featureIds.Count)
            .WithMessage("Vehicle features cannot contain duplicated values.");
        RuleFor(x => x.CreatedBy).NotEmpty().WithMessage("Created by is required.");
    }

    private sealed class CreateVehicleRateValidator : AbstractValidator<CreateVehicleRateRequest>
    {
        public CreateVehicleRateValidator()
        {
            RuleFor(x => x.RentalType)
                .IsInEnum()
                .WithMessage("Rental type is invalid.");

            RuleFor(x => x.Price)
                .GreaterThan(0)
                .WithMessage("Vehicle rate price must be greater than zero.");
        }
    }
}
