using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.BlockVehicleAvailability;

public sealed class BlockVehicleAvailabilityValidator : AbstractValidator<BlockVehicleAvailabilityCommand>
{
    public BlockVehicleAvailabilityValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty().WithMessage("Tenant Id is required.");
        RuleFor(x => x.VehicleId).NotEmpty().WithMessage("Vehicle Id is required.");
        RuleFor(x => x.StartDate).NotEmpty().WithMessage("Start date is required.");
        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("End date is required.")
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("End date must be greater than or equal to start date.");
        RuleFor(x => x.Reason).MaximumLength(250);
        RuleFor(x => x.CreatedBy).NotEmpty().WithMessage("Created by is required.");
    }
}
