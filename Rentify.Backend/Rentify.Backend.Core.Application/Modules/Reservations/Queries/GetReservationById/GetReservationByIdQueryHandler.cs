using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Reservations.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Reservations.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Queries;

public sealed class GetReservationByIdQueryHandler
    : IRequestHandler<GetReservationByIdQuery, ResultReponse<ReservationDetailsResponse>>
{
    private readonly IReservationRepository _reservationRepository;

    public GetReservationByIdQueryHandler(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<ResultReponse<ReservationDetailsResponse>> Handle(
        GetReservationByIdQuery request,
        CancellationToken cancellationToken)
    {
        ReservationDetailsResponse response =
            await _reservationRepository.GetDetailsAsync(
                request.TenantId,
                request.ReservationId,
                cancellationToken)
            ?? throw new ApiException(
                "Reserva no encontrada.",
                StatusCodes.Status404NotFound);

        return ResultReponse<ReservationDetailsResponse>.Success(response);
    }
}
