using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Domain.Entities.Vehicles;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Services;

public sealed class ReservationVehicleResolver : IReservationVehicleResolver
{
    private readonly IVehicleRepository _vehicleRepository;
    public ReservationVehicleResolver(IVehicleRepository vehicleRepository) => _vehicleRepository = vehicleRepository;
    public async Task<Vehicle> GetReservableVehicleAsync(Guid tenantId, Guid vehicleId, CancellationToken cancellationToken)
    {
        Vehicle vehicle = await _vehicleRepository.GetByIdAsync(tenantId, vehicleId, cancellationToken) ?? throw new ApiException("Vehículo no encontrado.", StatusCodes.Status404NotFound);
        if (!vehicle.IsActive || vehicle.Status is VehicleStatus.Maintenance or VehicleStatus.OutOfService) throw new ApiException("El vehículo no está disponible para reservas.", StatusCodes.Status400BadRequest);
        return vehicle;
    }
    public VehicleRate GetRateOrThrow(Vehicle vehicle, RentalType rentalType) => vehicle.Rates.FirstOrDefault(rate => rate.IsActive && !rate.IsDeleted && rate.RentalType == rentalType) ?? throw new ApiException("El vehículo no tiene una tarifa configurada para este tipo de renta.", StatusCodes.Status400BadRequest);
}
