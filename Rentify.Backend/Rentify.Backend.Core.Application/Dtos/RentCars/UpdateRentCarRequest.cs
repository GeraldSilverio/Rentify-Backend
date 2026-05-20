namespace Rentify.Backend.Core.Application.Dtos.RentCars;

public record UpdateRentCarRequest(
    string Name,
    string Description,
    string Phone,
    string Email,
    string WhatsApp,
    string Street,
    string City,
    string Country,
    string UpdatedBy);
