using Rentify.Backend.Core.Application.Modules.Reservations.Commands.CreateReservation;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Contracts.Services;

public interface IReservationService
{
    Task<Guid> CreateAsync(CreateReservationCommand command, CancellationToken cancellationToken = default);
}
