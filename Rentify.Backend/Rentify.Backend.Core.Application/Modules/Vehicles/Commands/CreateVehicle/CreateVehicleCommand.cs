using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.CreateVehicle;

public sealed record CreateVehicleCommand(
    Guid RentCarId,
    Guid ModelId,
    int Year,
    string PlateNumber,
    string Vin,
    string Color,
    decimal DailyRate,
    IFormFile Image,
    string CreatedBy) : IRequest<ResultReponse<Guid>>;
