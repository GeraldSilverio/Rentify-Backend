using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicleRateById;

public sealed class GetVehicleRateByIdValidator : AbstractValidator<GetVehicleRateByIdQuery>
{
    public GetVehicleRateByIdValidator()
    {
        RuleFor(query => query.TenantId).NotEmpty();
        RuleFor(query => query.VehicleId).NotEmpty();
        RuleFor(query => query.RateId).NotEmpty();
    }
}
