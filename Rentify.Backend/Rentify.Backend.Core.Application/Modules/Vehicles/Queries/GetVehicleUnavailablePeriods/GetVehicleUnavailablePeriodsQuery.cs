using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicleUnavailablePeriods;

public sealed record GetVehicleUnavailablePeriodsQuery(
    Guid TenantId,
    Guid VehicleId,
    DateOnly? FromDate,
    DateOnly? ToDate) : IRequest<ResultReponse<VehicleUnavailablePeriodsResponse>>;
