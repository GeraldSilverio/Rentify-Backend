using Rentify.Backend.Core.Application.Modules.RentCars.Dtos;
using Rentify.Backend.Core.Application.Shared.Dtos.Information;

namespace Rentify.Backend.Core.Application.Modules.RentCars.Commands.UpdateRentCar;

public sealed record UpdateRentCarRequest(
    string Name,
    string Description,
    AddressInformationDto AddressInformation,
    ContactInformationDto ContactInformation,
    string? LogoUrl,
    string ModifiedBy);
