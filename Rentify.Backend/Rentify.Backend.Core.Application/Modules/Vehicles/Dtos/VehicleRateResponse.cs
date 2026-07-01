using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;

public sealed record VehicleRateResponse(
    RentalType RentalType,
    decimal Price);
