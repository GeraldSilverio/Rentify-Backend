using MediatR;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetModels;

public sealed record GetModelsQuery(Guid VehicleBrandId) : IRequest<ResultReponse<IReadOnlyCollection<VehicleModelResponse>>>;
