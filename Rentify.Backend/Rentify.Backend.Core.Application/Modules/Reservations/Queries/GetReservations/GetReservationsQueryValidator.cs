using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Queries.GetReservations;

public sealed class GetReservationsQueryValidator : AbstractValidator<GetReservationsQuery>
{
    public GetReservationsQueryValidator()
    {
        RuleFor(query => query.TenantId)
            .NotEmpty();

        RuleFor(query => query.Search)
            .MaximumLength(100);

        RuleFor(query => query.Status)
            .IsInEnum()
            .When(query => query.Status.HasValue);

        RuleFor(query => query.PageNumber)
            .GreaterThan(0);

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(query => query.FromDate)
            .LessThanOrEqualTo(query => query.ToDate)
            .When(query => query.FromDate.HasValue && query.ToDate.HasValue)
            .WithMessage("La fecha inicial debe ser menor o igual que la fecha final.");
    }
}
