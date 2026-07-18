using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.AddVehicleRate;

public sealed record AddVehicleRateCommand(Guid TenantId, Guid VehicleId, RentalType RentalType, decimal Price, string CreatedBy)
    : IRequest<ResultReponse<VehicleRateResponse>>;

public sealed record AddVehicleRateRequest(RentalType RentalType, decimal Price);
