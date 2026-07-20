using Rentify.Backend.Core.Application.Modules.Reservations.Dtos;
using Rentify.Backend.Core.Domain.Entities.Reservations;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Mappers;

public static class ReservationMapper
{
    public static ReservationResponse ToResponse(this Reservation reservation)
    {
        return new ReservationResponse(reservation.Id, reservation.Code, reservation.Status, reservation.TotalAmount);
    }
}
