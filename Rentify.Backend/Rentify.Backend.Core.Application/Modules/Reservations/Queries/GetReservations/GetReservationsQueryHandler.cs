using MediatR;
using Rentify.Backend.Core.Application.Modules.Reservations.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Reservations.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Queries;

public sealed class GetReservationsQueryHandler
    : IRequestHandler<GetReservationsQuery, ResultReponse<PaginatedResponse<ReservationListItemResponse>>>
{
    private readonly IReservationRepository _reservationRepository;

    public GetReservationsQueryHandler(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<ResultReponse<PaginatedResponse<ReservationListItemResponse>>> Handle(
        GetReservationsQuery request,
        CancellationToken cancellationToken)
    {
        PaginatedResponse<ReservationListItemResponse> response =
            await _reservationRepository.GetPagedAsync(
                request,
                cancellationToken);

        return ResultReponse<PaginatedResponse<ReservationListItemResponse>>.Success(response);
    }
}
