using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.DeleteVehicleRate;

public sealed record DeleteVehicleRateCommand(Guid TenantId, Guid VehicleId, Guid RateId, string ModifiedBy)
    : IRequest<ResultReponse<bool>>;
