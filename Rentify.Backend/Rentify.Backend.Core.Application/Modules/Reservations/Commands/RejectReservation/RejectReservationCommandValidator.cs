using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Commands.RejectReservation;

public sealed class RejectReservationCommandValidator : AbstractValidator<RejectReservationCommand>
{
    public RejectReservationCommandValidator()
    {
        RuleFor(command => command.TenantId)
            .NotEmpty();

        RuleFor(command => command.ReservationId)
            .NotEmpty();

        RuleFor(command => command.Reason)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(command => command.RejectedBy)
            .NotEmpty()
            .MaximumLength(150);
    }
}
