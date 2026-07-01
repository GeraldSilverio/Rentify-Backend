using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UploadVehicleImage;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicleImages;

public sealed record GetVehicleImagesQuery(
    Guid TenantId,
    Guid VehicleId) : IRequest<ResultReponse<IReadOnlyCollection<VehicleImageResponse>>>;
