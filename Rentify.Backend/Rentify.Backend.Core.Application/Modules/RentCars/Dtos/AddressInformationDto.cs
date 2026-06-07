namespace Rentify.Backend.Core.Application.Modules.RentCars.Dtos;

public record AddressInformationDto(
    string Street,
    string City,
    string Country = "Republica Dominicana");