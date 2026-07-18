using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;

public sealed record VehicleListItemResponse(
    Guid Id,
    Guid TenantId,
    Guid VehicleBrandId,
    string VehicleBrandName,
    Guid VehicleModelId,
    string VehicleModelName,
    Guid VehicleTypeId,
    string VehicleTypeName,
    int Year,
    string PlateNumber,
    string Color,
    int? CurrentMileage,
    bool SecurityDepositRequired,
    decimal SecurityDepositAmount,
    VehicleStatus Status,
    bool IsActive,
    string? PrimaryImageUrl,
    IReadOnlyList<VehicleRateResponse> Rates,
    int FeaturesCount,
    DateTime CreatedDate);
