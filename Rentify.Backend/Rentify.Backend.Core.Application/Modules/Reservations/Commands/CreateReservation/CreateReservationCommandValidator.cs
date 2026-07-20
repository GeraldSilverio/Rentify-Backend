using FluentValidation;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Commands.CreateReservation;

public sealed class CreateReservationCommandValidator : AbstractValidator<CreateReservationCommand>
{
    public CreateReservationCommandValidator()
    {
        RuleFor(command => command.TenantId)
            .NotEmpty();

        RuleFor(command => command.CustomerId)
            .NotEmpty();

        RuleFor(command => command.VehicleId)
            .NotEmpty();

        RuleFor(command => command.DeliveryDateTime)
            .LessThan(command => command.ExpectedReturnDateTime)
            .WithMessage("La fecha de entrega debe ser menor que la fecha de devolución.");

        RuleFor(command => command.RentalType)
            .IsInEnum();

        RuleFor(command => command.Channel)
            .IsInEnum();

        RuleFor(command => command.DeliveryLocationName)
            .NotEmpty()
            .MaximumLength(150)
            .When(command => !command.DeliveryTenantLocationId.HasValue);

        RuleFor(command => command.DeliveryLocationName)
            .MaximumLength(150)
            .When(command => command.DeliveryTenantLocationId.HasValue);

        RuleFor(command => command.DeliveryAddressDetails)
            .MaximumLength(500);

        RuleFor(command => command.DeliveryFee)
            .GreaterThanOrEqualTo(0);

        RuleFor(command => command.ReturnLocationName)
            .NotEmpty()
            .MaximumLength(150)
            .When(command => !command.ReturnTenantLocationId.HasValue);

        RuleFor(command => command.ReturnLocationName)
            .MaximumLength(150)
            .When(command => command.ReturnTenantLocationId.HasValue);

        RuleFor(command => command.ReturnAddressDetails)
            .MaximumLength(500);

        RuleFor(command => command.ReturnFee)
            .GreaterThanOrEqualTo(0);

        RuleFor(command => command.DiscountAmount)
            .GreaterThanOrEqualTo(0);

        RuleFor(command => command.Notes)
            .MaximumLength(1000);

        RuleFor(command => command.CreatedBy)
            .NotEmpty()
            .MaximumLength(150);
    }
}
