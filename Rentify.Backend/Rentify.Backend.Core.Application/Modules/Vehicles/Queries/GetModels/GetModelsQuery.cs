using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetModels;

public sealed record GetModelsQuery(Guid VehicleBrandId) : IRequest<ResultReponse<IReadOnlyCollection<VehicleModelResponse>>>;
