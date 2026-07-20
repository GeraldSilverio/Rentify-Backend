namespace Rentify.Backend.Core.Application.Modules.Reservations.Contracts.Services;

public interface IReservationCustomerValidator
{
    Task ValidateAsync(
        Guid tenantId,
        Guid customerId,
        CancellationToken cancellationToken);
}
