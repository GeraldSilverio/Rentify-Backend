using MediatR;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.BlockVehicleAvailability;

public sealed record BlockVehicleAvailabilityCommand(
    Guid TenantId,
    Guid VehicleId,
    DateOnly StartDate,
    DateOnly EndDate,
    string? Reason,
    string CreatedBy) : IRequest<ResultReponse<bool>>;
