using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Commands.DeleteReservation;

public sealed class DeleteReservationCommandValidator : AbstractValidator<DeleteReservationCommand>
{
    public DeleteReservationCommandValidator()
    {
        RuleFor(command => command.TenantId)
            .NotEmpty();

        RuleFor(command => command.ReservationId)
            .NotEmpty();

        RuleFor(command => command.ModifiedBy)
            .NotEmpty()
            .MaximumLength(150);
    }
}
