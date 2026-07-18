using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicleRateById;

public sealed record GetVehicleRateByIdQuery(Guid TenantId, Guid VehicleId, Guid RateId)
    : IRequest<ResultReponse<VehicleRateResponse>>;
