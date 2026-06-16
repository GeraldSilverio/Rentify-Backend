using MediatR;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.CreateVehicle;

public sealed record CreateVehicleCommand(
    Guid TenantId,
    Guid RentCarId,
    Guid VehicleModelId,
    Guid VehicleTypeId,
    int Year,
    string PlateNumber,
    string Vin,
    string Color,
    decimal DailyRate,
    string CreatedBy) : IRequest<ResultReponse<Guid>>;
