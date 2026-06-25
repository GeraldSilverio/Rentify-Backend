using MediatR;
using Rentify.Backend.Core.Application.Common.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;
using Rentify.Backend.Core.Application.Shared.Response;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicles;

public sealed record GetVehiclesQuery(
    Guid TenantId,
    int PageNumber = 1,
    int PageSize = 10,
    string? Search = null,
    Guid? VehicleTypeId = null,
    Guid? VehicleBrandId = null,
    Guid? VehicleModelId = null,
    VehicleStatus? Status = null,
    decimal? MinDailyRate = null,
    decimal? MaxDailyRate = null,
    bool? OnlyActive = true) : IRequest<ResultReponse<PaginatedResponse<VehicleListItemResponse>>>;
