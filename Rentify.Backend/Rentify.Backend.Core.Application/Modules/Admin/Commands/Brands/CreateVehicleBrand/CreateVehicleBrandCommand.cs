using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Admin.Commands.Brands.CreateVehicleBrand
{
    public sealed record CreateVehicleBrandCommand(string BrandName) : IRequest<ResultReponse<CreateVehicleBrandResponse>>
    {

    }
}
