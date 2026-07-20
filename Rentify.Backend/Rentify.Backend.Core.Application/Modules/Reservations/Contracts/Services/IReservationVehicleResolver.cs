using Rentify.Backend.Core.Domain.Entities.Vehicles;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Contracts.Services;

public interface IReservationVehicleResolver
{
    Task<Vehicle> GetReservableVehicleAsync(
        Guid tenantId,
        Guid vehicleId,
        CancellationToken cancellationToken);

    VehicleRate GetRateOrThrow(
        Vehicle vehicle,
        RentalType rentalType);
}
