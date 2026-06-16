using MediatR;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.SetPrimaryVehicleImage;

public sealed record SetPrimaryVehicleImageCommand(
    Guid TenantId,
    Guid VehicleId,
    Guid ImageId,
    string ModifiedBy) : IRequest<ResultReponse<bool>>;
