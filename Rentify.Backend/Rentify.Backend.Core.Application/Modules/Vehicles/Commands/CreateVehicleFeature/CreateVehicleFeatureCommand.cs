using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.CreateVehicleFeature;

public sealed record CreateVehicleFeatureCommand(
    string Name,
    string Category,
    string CreatedBy) : IRequest<ResultReponse<VehicleFeatureResponse>>;
