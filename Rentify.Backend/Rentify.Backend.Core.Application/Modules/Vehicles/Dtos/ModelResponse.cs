namespace Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;

public sealed record ModelResponse(Guid Id, string Name, Guid BrandId, string BrandName);
