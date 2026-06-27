using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.DeleteVehicle;

public sealed record DeleteVehicleCommand(
    Guid TenantId,
    Guid VehicleId,
    string ModifiedBy) : IRequest<ResultReponse<bool>>;
