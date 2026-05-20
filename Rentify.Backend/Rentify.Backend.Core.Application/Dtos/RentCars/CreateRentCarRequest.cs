namespace Rentify.Backend.Core.Application.Dtos.RentCars;

public record CreateRentCarRequest(
    string Name,
    string Description,
    string Phone,
    string Email,
    string WhatsApp,
    string Street,
    string City,
    string Country,
    string CreatedBy);
