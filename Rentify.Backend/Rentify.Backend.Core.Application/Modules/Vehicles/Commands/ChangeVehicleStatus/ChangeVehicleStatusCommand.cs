using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.ChangeVehicleStatus;

public sealed record ChangeVehicleStatusCommand(
    Guid TenantId,
    Guid VehicleId,
    VehicleStatus Status,
    string ModifiedBy) : IRequest<ResultReponse<bool>>;
