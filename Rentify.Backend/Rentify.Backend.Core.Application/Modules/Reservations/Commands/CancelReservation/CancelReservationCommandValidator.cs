using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Commands.CancelReservation;

public sealed class CancelReservationCommandValidator : AbstractValidator<CancelReservationCommand>
{
    public CancelReservationCommandValidator()
    {
        RuleFor(command => command.TenantId)
            .NotEmpty();

        RuleFor(command => command.ReservationId)
            .NotEmpty();

        RuleFor(command => command.Reason)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(command => command.CancelledBy)
            .NotEmpty()
            .MaximumLength(150);
    }
}
