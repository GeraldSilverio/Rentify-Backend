using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Queries.CheckVehicleReservationAvailability;

public sealed class CheckVehicleReservationAvailabilityQueryValidator
    : AbstractValidator<CheckVehicleReservationAvailabilityQuery>
{
    public CheckVehicleReservationAvailabilityQueryValidator()
    {
        RuleFor(query => query.TenantId)
            .NotEmpty();

        RuleFor(query => query.VehicleId)
            .NotEmpty();

        RuleFor(query => query.DeliveryDateTime)
            .LessThan(query => query.ExpectedReturnDateTime)
            .WithMessage("La fecha de entrega debe ser menor que la fecha de devolución.");
    }
}
