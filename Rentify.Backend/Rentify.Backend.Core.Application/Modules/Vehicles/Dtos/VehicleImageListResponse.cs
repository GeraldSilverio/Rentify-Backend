namespace Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;

public sealed record VehicleImageListResponse(
    Guid Id,
    string Url,
    string PublicId,
    bool IsPrimary,
    DateTime CreatedDate);
