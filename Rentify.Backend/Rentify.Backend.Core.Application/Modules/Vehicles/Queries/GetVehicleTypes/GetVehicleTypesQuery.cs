using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicleTypes;

public sealed record GetVehicleTypesQuery(
    bool OnlyActive = true) : IRequest<ResultReponse<IReadOnlyCollection<VehicleTypeResponse>>>;
