namespace Rentify.Backend.Core.Application.Modules.Reservations.Contracts.Services;

public interface IReservationCodeGenerator
{
    Task<string> GenerateAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
