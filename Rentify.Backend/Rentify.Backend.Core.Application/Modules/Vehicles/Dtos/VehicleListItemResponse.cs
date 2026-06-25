using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;

public sealed record VehicleListItemResponse(
    Guid Id,
    Guid TenantId,
    Guid VehicleModelId,
    string VehicleModelName,
    Guid VehicleBrandId,
    string VehicleBrandName,
    Guid VehicleTypeId,
    string VehicleTypeName,
    int Year,
    string PlateNumber,
    string Vin,
    string Color,
    decimal DailyRate,
    VehicleStatus Status,
    bool IsActive,
    IReadOnlyList<VehicleImageListResponse> Images,
    DateTime CreatedDate,
    DateTime ModifiedDate);
