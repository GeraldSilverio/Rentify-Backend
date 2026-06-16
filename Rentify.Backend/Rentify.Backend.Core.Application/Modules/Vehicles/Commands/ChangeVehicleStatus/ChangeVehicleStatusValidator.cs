using FluentValidation;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.ChangeVehicleStatus;

public sealed class ChangeVehicleStatusValidator : AbstractValidator<ChangeVehicleStatusCommand>
{
    public ChangeVehicleStatusValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty().WithMessage("Tenant Id is required.");
        RuleFor(x => x.VehicleId).NotEmpty().WithMessage("Vehicle Id is required.");
        RuleFor(x => x.Status).IsInEnum().NotEqual((VehicleStatus)0).WithMessage("Vehicle status is invalid.");
        RuleFor(x => x.ModifiedBy).NotEmpty().WithMessage("Modified by is required.");
    }
}
