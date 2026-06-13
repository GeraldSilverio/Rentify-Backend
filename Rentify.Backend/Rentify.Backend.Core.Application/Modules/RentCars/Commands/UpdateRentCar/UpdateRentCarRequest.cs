using Rentify.Backend.Core.Application.Modules.RentCars.Dtos;

namespace Rentify.Backend.Core.Application.Modules.RentCars.Commands.UpdateRentCar;

public sealed record UpdateRentCarRequest(
    string Name,
    string Description,
    AddressInformationDto AddressInformation,
    ContactInfomationDto ContactInformation,
    string? LogoUrl,
    string ModifiedBy);
