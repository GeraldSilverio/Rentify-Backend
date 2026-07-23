using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicleUnavailablePeriods;

public sealed class GetVehicleUnavailablePeriodsQueryValidator
    : AbstractValidator<GetVehicleUnavailablePeriodsQuery>
{
    public GetVehicleUnavailablePeriodsQueryValidator()
    {
        RuleFor(query => query.TenantId)
            .NotEmpty();

        RuleFor(query => query.VehicleId)
            .NotEmpty();

        When(
            query => query.FromDate.HasValue && query.ToDate.HasValue,
            () => RuleFor(query => query.ToDate!.Value)
                .GreaterThan(query => query.FromDate!.Value)
                .WithMessage("La fecha final debe ser posterior a la fecha inicial."));
    }
}
