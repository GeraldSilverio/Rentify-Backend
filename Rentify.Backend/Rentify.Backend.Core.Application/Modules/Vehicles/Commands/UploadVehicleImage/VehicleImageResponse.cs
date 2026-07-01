namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UploadVehicleImage;

public sealed record VehicleImageResponse(
    Guid Id,
    string Url,
    string PublicId,
    bool IsPrimary,
    DateTime CreatedDate);
