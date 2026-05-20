namespace Rentify.Backend.Core.Application.Dtos.RentCars;

public record RentCarResponse(
    Guid Id,
    string Name,
    string Description,
    string Phone,
    string Email,
    string WhatsApp,
    string Street,
    string City,
    string Country,
    bool IsActive,
    DateTime CreatedDate,
    DateTime ModifiedDate,
    string CreatedBy,
    string ModifiedBy);