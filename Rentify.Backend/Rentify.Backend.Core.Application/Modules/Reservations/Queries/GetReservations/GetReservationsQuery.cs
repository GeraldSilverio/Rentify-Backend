using MediatR;
using Rentify.Backend.Core.Application.Modules.Reservations.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Queries;

public sealed record GetReservationsQuery(
    Guid TenantId,
    string? Search,
    ReservationStatus? Status,
    Guid? CustomerId,
    Guid? VehicleId,
    DateTime? FromDate,
    DateTime? ToDate,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<ResultReponse<PaginatedResponse<ReservationListItemResponse>>>;
