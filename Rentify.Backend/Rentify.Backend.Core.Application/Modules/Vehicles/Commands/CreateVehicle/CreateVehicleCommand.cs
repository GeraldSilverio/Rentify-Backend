using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.CreateVehicle;

public sealed record CreateVehicleCommand(
    Guid TenantId,
    Guid VehicleBrandId,
    Guid VehicleModelId,
    Guid VehicleTypeId,
    int Year,
    string PlateNumber,
    string? Vin,
    string Color,
    int? CurrentMileage,
    IReadOnlyCollection<CreateVehicleRateRequest> Rates,
    IReadOnlyCollection<Guid> FeatureIds,
    string CreatedBy) : IRequest<ResultReponse<CreateVehicleResponse>>;

public sealed record CreateVehicleRateRequest(
    RentalType RentalType,
    decimal Price);

public sealed record CreateVehicleRateResponse(
    RentalType RentalType,
    decimal Price);

public sealed record CreateVehicleResponse(
    Guid Id,
    Guid TenantId,
    Guid VehicleBrandId,
    Guid VehicleModelId,
    Guid VehicleTypeId,
    int Year,
    string PlateNumber,
    string? Vin,
    string Color,
    int? CurrentMileage,
    IReadOnlyCollection<CreateVehicleRateResponse> Rates,
    IReadOnlyCollection<Guid> FeatureIds,
    VehicleStatus Status,
    bool IsActive,
    DateTime CreatedDate);
