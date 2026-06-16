using MediatR;
using Rentify.Backend.Core.Application.Modules.Reservations.Contracts.Services;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Commands.CreateReservation;

public sealed class CreateReservationHandler : IRequestHandler<CreateReservationCommand, ResultReponse<Guid>>
{
    private readonly IReservationService _reservationService;

    public CreateReservationHandler(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    public async Task<ResultReponse<Guid>> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        Guid reservationId = await _reservationService.CreateAsync(request, cancellationToken);
        return ResultReponse<Guid>.Success(reservationId);
    }
}
