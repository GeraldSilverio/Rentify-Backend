namespace Rentify.Backend.Core.Application.Modules.Reservations.Services;

public interface IReservationCustomerValidator
{
    Task ValidateAsync(Guid tenantId, Guid customerId, CancellationToken cancellationToken);
}
