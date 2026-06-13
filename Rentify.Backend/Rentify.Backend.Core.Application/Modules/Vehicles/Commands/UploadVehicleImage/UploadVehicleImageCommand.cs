using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UploadVehicleImage;

public sealed record UploadVehicleImageCommand(
    Guid VehicleId,
    IFormFile Image,
    bool IsPrimary,
    string CreatedBy) : IRequest<ResultReponse<Guid>>;
