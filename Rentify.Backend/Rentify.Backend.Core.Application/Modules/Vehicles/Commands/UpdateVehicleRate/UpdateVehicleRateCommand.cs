using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UpdateVehicleRate;

public sealed record UpdateVehicleRateCommand(Guid TenantId, Guid VehicleId, Guid RateId, decimal Price, string ModifiedBy)
    : IRequest<ResultReponse<VehicleRateResponse>>;

public sealed record UpdateVehicleRateRequest(decimal Price);
