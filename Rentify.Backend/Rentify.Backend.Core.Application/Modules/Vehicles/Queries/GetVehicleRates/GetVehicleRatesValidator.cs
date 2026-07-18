using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicleRates;

public sealed class GetVehicleRatesValidator : AbstractValidator<GetVehicleRatesQuery>
{
    public GetVehicleRatesValidator()
    {
        RuleFor(query => query.TenantId).NotEmpty();
        RuleFor(query => query.VehicleId).NotEmpty();
    }
}
