using MediatR;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.DeleteVehicleImage;

public sealed record DeleteVehicleImageCommand(
    Guid VehicleId,
    Guid ImageId,
    string ModifiedBy) : IRequest<ResultReponse<bool>>;
