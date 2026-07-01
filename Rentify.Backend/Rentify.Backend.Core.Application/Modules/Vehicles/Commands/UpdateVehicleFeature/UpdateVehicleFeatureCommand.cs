using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UpdateVehicleFeature;

public sealed record UpdateVehicleFeatureCommand(
    Guid FeatureId,
    string Name,
    string Category,
    string ModifiedBy) : IRequest<ResultReponse<VehicleFeatureResponse>>;
