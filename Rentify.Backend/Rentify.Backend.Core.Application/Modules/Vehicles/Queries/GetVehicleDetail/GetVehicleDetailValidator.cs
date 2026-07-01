using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicleDetail;

public sealed class GetVehicleDetailValidator : AbstractValidator<GetVehicleDetailQuery>
{
    public GetVehicleDetailValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.VehicleId).NotEmpty();
    }
}
