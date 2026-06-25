namespace Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;

public sealed record CurrentRentalResponse(
    bool HasActiveRental,
    Guid? ReservationId,
    Guid? CustomerId,
    string? CustomerName,
    DateOnly? StartDate,
    DateOnly? EndDate);
