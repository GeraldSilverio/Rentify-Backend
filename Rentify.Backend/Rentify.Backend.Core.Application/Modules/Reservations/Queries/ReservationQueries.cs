using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Reservations.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Reservations.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Queries;

public sealed record GetReservationsQuery(Guid TenantId, string? Search, ReservationStatus? Status, Guid? CustomerId, Guid? VehicleId, DateTime? FromDate, DateTime? ToDate, int PageNumber = 1, int PageSize = 10) : IRequest<ResultReponse<PaginatedResponse<ReservationListItemResponse>>>;
public sealed record GetReservationByIdQuery(Guid TenantId, Guid ReservationId) : IRequest<ResultReponse<ReservationDetailsResponse>>;
public sealed record CheckVehicleReservationAvailabilityQuery(Guid TenantId, Guid VehicleId, DateTime DeliveryDateTime, DateTime ExpectedReturnDateTime, Guid? ExcludeReservationId) : IRequest<ResultReponse<ReservationAvailabilityResponse>>;
public sealed class GetReservationsQueryValidator : AbstractValidator<GetReservationsQuery> { public GetReservationsQueryValidator() { RuleFor(x => x.TenantId).NotEmpty(); RuleFor(x => x.PageNumber).GreaterThan(0); RuleFor(x => x.PageSize).InclusiveBetween(1, 100); RuleFor(x => x.Search).MaximumLength(100); } }
public sealed class GetReservationByIdQueryValidator : AbstractValidator<GetReservationByIdQuery> { public GetReservationByIdQueryValidator() { RuleFor(x => x.TenantId).NotEmpty(); RuleFor(x => x.ReservationId).NotEmpty(); } }
public sealed class CheckVehicleReservationAvailabilityQueryValidator : AbstractValidator<CheckVehicleReservationAvailabilityQuery> { public CheckVehicleReservationAvailabilityQueryValidator() { RuleFor(x => x.TenantId).NotEmpty(); RuleFor(x => x.VehicleId).NotEmpty(); RuleFor(x => x.DeliveryDateTime).LessThan(x => x.ExpectedReturnDateTime); } }
public sealed class ReservationQueryHandlers : IRequestHandler<GetReservationsQuery, ResultReponse<PaginatedResponse<ReservationListItemResponse>>>, IRequestHandler<GetReservationByIdQuery, ResultReponse<ReservationDetailsResponse>>, IRequestHandler<CheckVehicleReservationAvailabilityQuery, ResultReponse<ReservationAvailabilityResponse>>
{
    private readonly IReservationRepository _reservations; public ReservationQueryHandlers(IReservationRepository reservations) => _reservations = reservations;
    public async Task<ResultReponse<PaginatedResponse<ReservationListItemResponse>>> Handle(GetReservationsQuery request, CancellationToken cancellationToken) => ResultReponse<PaginatedResponse<ReservationListItemResponse>>.Success(await _reservations.GetPagedAsync(request, cancellationToken));
    public async Task<ResultReponse<ReservationDetailsResponse>> Handle(GetReservationByIdQuery request, CancellationToken cancellationToken) => ResultReponse<ReservationDetailsResponse>.Success(await _reservations.GetDetailsAsync(request.TenantId, request.ReservationId, cancellationToken) ?? throw new ApiException("Reserva no encontrada.", StatusCodes.Status404NotFound));
    public async Task<ResultReponse<ReservationAvailabilityResponse>> Handle(CheckVehicleReservationAvailabilityQuery request, CancellationToken cancellationToken) { bool occupied = await _reservations.HasApprovedOverlapAsync(request.TenantId, request.VehicleId, request.DeliveryDateTime, request.ExpectedReturnDateTime, request.ExcludeReservationId, cancellationToken); return ResultReponse<ReservationAvailabilityResponse>.Success(new ReservationAvailabilityResponse(!occupied, occupied ? "El vehÃ­culo ya tiene una reserva aprobada para el rango de fechas seleccionado." : null)); }
}
