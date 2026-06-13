using MediatR;
using Rentify.Backend.Core.Application.Modules.RentCars.Dtos;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.RentCars.Commands.UpdateRentCar;

public sealed record UpdateRentCarCommand(
    Guid Id,
    string Name,
    string Description,
    AddressInformationDto AddressInformation,
    ContactInfomationDto ContactInformation,
    string? LogoUrl,
    string ModifiedBy) : IRequest<ResultReponse<Guid>>;
