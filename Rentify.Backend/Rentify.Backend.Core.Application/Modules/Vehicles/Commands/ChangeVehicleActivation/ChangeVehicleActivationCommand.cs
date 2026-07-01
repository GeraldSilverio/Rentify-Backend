using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.ChangeVehicleActivation;

public sealed record ChangeVehicleActivationCommand(
    Guid TenantId,
    Guid VehicleId,
    bool IsActive,
    string ModifiedBy) : IRequest<ResultReponse<bool>>;
