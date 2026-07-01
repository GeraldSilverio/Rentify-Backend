using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicleDetail;

public sealed record GetVehicleDetailQuery(
    Guid TenantId,
    Guid VehicleId) : IRequest<ResultReponse<VehicleDetailResponse>>;
