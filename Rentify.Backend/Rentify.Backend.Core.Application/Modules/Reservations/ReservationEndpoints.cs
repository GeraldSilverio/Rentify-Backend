using Microsoft.AspNetCore.Routing;
using Rentify.Backend.Core.Application.Modules.Reservations.Commands.CreateReservation;

namespace Rentify.Backend.Core.Application.Modules.Reservations;

public static class ReservationEndpoints
{
    public static IEndpointRouteBuilder MapReservationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCreateReservationEndpoint();
        return app;
    }
}
