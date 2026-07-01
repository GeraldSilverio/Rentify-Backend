using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetBrands;

public sealed record GetBrandsQuery(
    bool OnlyActive = true) : IRequest<ResultReponse<IReadOnlyCollection<VehicleBrandResponse>>>;
