using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Commands;

public sealed class CreateReservationCommandValidator : AbstractValidator<CreateReservationCommand>
{
    public CreateReservationCommandValidator() { ConfigureCommon(); RuleFor(x => x.CustomerId).NotEmpty(); RuleFor(x => x.VehicleId).NotEmpty(); RuleFor(x => x.Channel).IsInEnum(); }
    private void ConfigureCommon() { RuleFor(x => x.TenantId).NotEmpty(); RuleFor(x => x.DeliveryDateTime).LessThan(x => x.ExpectedReturnDateTime); RuleFor(x => x.RentalType).IsInEnum(); RuleFor(x => x.DeliveryLocationName).NotEmpty().MaximumLength(150).When(x => !x.DeliveryTenantLocationId.HasValue); RuleFor(x => x.ReturnLocationName).NotEmpty().MaximumLength(150).When(x => !x.ReturnTenantLocationId.HasValue); RuleFor(x => x.DeliveryAddressDetails).MaximumLength(500); RuleFor(x => x.ReturnAddressDetails).MaximumLength(500); RuleFor(x => x.DeliveryFee).GreaterThanOrEqualTo(0); RuleFor(x => x.ReturnFee).GreaterThanOrEqualTo(0); RuleFor(x => x.DiscountAmount).GreaterThanOrEqualTo(0); RuleFor(x => x.Notes).MaximumLength(1000); }
}
public sealed class UpdateReservationCommandValidator : AbstractValidator<UpdateReservationCommand>
{
    public UpdateReservationCommandValidator() { RuleFor(x => x.TenantId).NotEmpty(); RuleFor(x => x.ReservationId).NotEmpty(); RuleFor(x => x.DeliveryDateTime).LessThan(x => x.ExpectedReturnDateTime); RuleFor(x => x.RentalType).IsInEnum(); RuleFor(x => x.DeliveryLocationName).NotEmpty().MaximumLength(150).When(x => !x.DeliveryTenantLocationId.HasValue); RuleFor(x => x.ReturnLocationName).NotEmpty().MaximumLength(150).When(x => !x.ReturnTenantLocationId.HasValue); RuleFor(x => x.DeliveryAddressDetails).MaximumLength(500); RuleFor(x => x.ReturnAddressDetails).MaximumLength(500); RuleFor(x => x.DeliveryFee).GreaterThanOrEqualTo(0); RuleFor(x => x.ReturnFee).GreaterThanOrEqualTo(0); RuleFor(x => x.DiscountAmount).GreaterThanOrEqualTo(0); RuleFor(x => x.Notes).MaximumLength(1000); }
}
public sealed class ApproveReservationCommandValidator : AbstractValidator<ApproveReservationCommand> { public ApproveReservationCommandValidator() { RuleFor(x => x.TenantId).NotEmpty(); RuleFor(x => x.ReservationId).NotEmpty(); } }
public sealed class RejectReservationCommandValidator : AbstractValidator<RejectReservationCommand> { public RejectReservationCommandValidator() { RuleFor(x => x.TenantId).NotEmpty(); RuleFor(x => x.ReservationId).NotEmpty(); RuleFor(x => x.Reason).NotEmpty().MaximumLength(500); } }
public sealed class CancelReservationCommandValidator : AbstractValidator<CancelReservationCommand> { public CancelReservationCommandValidator() { RuleFor(x => x.TenantId).NotEmpty(); RuleFor(x => x.ReservationId).NotEmpty(); RuleFor(x => x.Reason).NotEmpty().MaximumLength(500); } }
public sealed class DeleteReservationCommandValidator : AbstractValidator<DeleteReservationCommand> { public DeleteReservationCommandValidator() { RuleFor(x => x.TenantId).NotEmpty(); RuleFor(x => x.ReservationId).NotEmpty(); } }
