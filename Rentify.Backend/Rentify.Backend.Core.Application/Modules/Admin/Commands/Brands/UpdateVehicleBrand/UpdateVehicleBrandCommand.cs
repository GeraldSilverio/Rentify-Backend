using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Admin.Commands.Brands.UpdateVehicleBrand
{
    public sealed record UpdateVehicleBrandCommand(Guid BrandId, string BrandName) : IRequest<ResultReponse<UpdateVehicleBrandResponse>>;
}
