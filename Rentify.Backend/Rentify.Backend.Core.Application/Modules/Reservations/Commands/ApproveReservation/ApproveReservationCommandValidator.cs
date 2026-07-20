using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Commands.ApproveReservation;

public sealed class ApproveReservationCommandValidator : AbstractValidator<ApproveReservationCommand>
{
    public ApproveReservationCommandValidator()
    {
        RuleFor(command => command.TenantId)
            .NotEmpty();

        RuleFor(command => command.ReservationId)
            .NotEmpty();

        RuleFor(command => command.ApprovedBy)
            .NotEmpty()
            .MaximumLength(150);
    }
}
