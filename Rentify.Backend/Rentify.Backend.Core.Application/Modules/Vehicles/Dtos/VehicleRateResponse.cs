using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;

public sealed record VehicleRateResponse(Guid Id, RentalType RentalType, decimal Price)
{
    public VehicleRateResponse(RentalType rentalType, decimal price)
        : this(Guid.Empty, rentalType, price)
    {
    }
}
