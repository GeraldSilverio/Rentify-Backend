using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Queries.GetReservationById;

public sealed class GetReservationByIdQueryValidator : AbstractValidator<GetReservationByIdQuery>
{
    public GetReservationByIdQueryValidator()
    {
        RuleFor(query => query.TenantId)
            .NotEmpty();

        RuleFor(query => query.ReservationId)
            .NotEmpty();
    }
}
