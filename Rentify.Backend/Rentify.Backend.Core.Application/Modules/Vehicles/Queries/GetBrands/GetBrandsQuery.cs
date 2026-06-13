using MediatR;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetBrands;

public sealed record GetBrandsQuery : IRequest<ResultReponse<IReadOnlyCollection<BrandResponse>>>;
