using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.ChangeVehicleFeatureStatus;

public sealed record ChangeVehicleFeatureStatusCommand(
    Guid FeatureId,
    bool IsActive,
    string ModifiedBy) : IRequest<ResultReponse<VehicleFeatureResponse>>;
