using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.CreateVehicle;

public sealed record CreateVehicleCommand(
    Guid TenantId,
    Guid RentCarId,
    Guid VehicleBrandId,
    Guid VehicleModelId,
    Guid VehicleTypeId,
    int Year,
    string PlateNumber,
    string? Vin,
    string Color,
    decimal DailyRate,
    int? CurrentMileage,
    string CreatedBy) : IRequest<ResultReponse<Guid>>;
