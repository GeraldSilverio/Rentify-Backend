using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;

public sealed record VehicleDetailResponse(
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
    string? Vin,
    string Color,
    int? CurrentMileage,
    VehicleStatus Status,
    bool IsActive,
    IReadOnlyList<VehicleRateResponse> Rates,
    IReadOnlyList<VehicleImageListResponse> Images,
    IReadOnlyList<VehicleFeatureResponse> Features,
    DateTime CreatedDate,
    DateTime ModifiedDate);
