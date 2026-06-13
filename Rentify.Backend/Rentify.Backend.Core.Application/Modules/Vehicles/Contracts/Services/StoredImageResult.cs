namespace Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Services;

public sealed record StoredImageResult(
    string Url,
    string FileName,
    string ContentType,
    long SizeInBytes);
