using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Commands.CreateReservation;

public sealed class CreateReservationValidator : AbstractValidator<CreateReservationCommand>
{
    public CreateReservationValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.VehicleIds).NotEmpty().WithMessage("At least one vehicle is required.");
        RuleForEach(x => x.VehicleIds).NotEmpty();
        RuleFor(x => x.StartDate).GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow));
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate);
        RuleFor(x => x.CreatedBy).NotEmpty();
    }
}
