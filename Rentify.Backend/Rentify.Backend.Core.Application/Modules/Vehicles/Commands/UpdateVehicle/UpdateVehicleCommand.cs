using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UpdateVehicle;

public sealed record UpdateVehicleCommand(
    Guid TenantId,
    Guid VehicleId,
    Guid VehicleBrandId,
    Guid VehicleModelId,
    Guid VehicleTypeId,
    int Year,
    string PlateNumber,
    string? Vin,
    string Color,
    decimal DailyRate,
    int? CurrentMileage,
    string ModifiedBy) : IRequest<ResultReponse<Guid>>;
