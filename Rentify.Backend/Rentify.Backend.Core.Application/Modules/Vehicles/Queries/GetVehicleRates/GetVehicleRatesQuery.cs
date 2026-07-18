using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicleRates;

public sealed record GetVehicleRatesQuery(Guid TenantId, Guid VehicleId)
    : IRequest<ResultReponse<IReadOnlyCollection<VehicleRateResponse>>>;
