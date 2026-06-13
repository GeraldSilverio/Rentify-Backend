using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.ChangeVehicleStatus;

public sealed record ChangeVehicleStatusRequest(VehicleStatus Status, string ModifiedBy);
