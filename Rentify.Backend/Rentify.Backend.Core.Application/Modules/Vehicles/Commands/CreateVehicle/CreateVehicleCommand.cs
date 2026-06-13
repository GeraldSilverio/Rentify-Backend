using MediatR;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.CreateVehicle;

public sealed record CreateVehicleCommand(
    Guid RentCarId,
    string Make,
    string Model,
    int Year,
    string PlateNumber,
    string Vin,
    string Color,
    decimal DailyRate,
    string CreatedBy) : IRequest<ResultReponse<Guid>>;
