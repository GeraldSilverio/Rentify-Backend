using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicles;

public sealed class GetVehiclesValidator : AbstractValidator<GetVehiclesQuery>
{
    public GetVehiclesValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.Search).MaximumLength(100).When(x => x.Search is not null);
        RuleFor(x => x.Year)
            .InclusiveBetween(1980, DateTime.UtcNow.Year + 1)
            .When(x => x.Year.HasValue)
            .WithMessage("Vehicle year is invalid.");
        RuleFor(x => x.MinDailyRate).GreaterThanOrEqualTo(0).When(x => x.MinDailyRate.HasValue);
        RuleFor(x => x.MaxDailyRate).GreaterThanOrEqualTo(0).When(x => x.MaxDailyRate.HasValue);
        RuleFor(x => x)
            .Must(x => !x.MinDailyRate.HasValue || !x.MaxDailyRate.HasValue || x.MinDailyRate <= x.MaxDailyRate)
            .WithMessage("Minimum daily rate cannot be greater than maximum daily rate.");
    }
}
