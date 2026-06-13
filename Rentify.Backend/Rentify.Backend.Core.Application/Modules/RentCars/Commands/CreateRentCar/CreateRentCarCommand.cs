using MediatR;
using Rentify.Backend.Core.Application.Modules.RentCars.Dtos;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.RentCars.Commands.CreateRentCar;

public sealed record CreateRentCarCommand(
    string Name,
    string Description,
    Guid TenantId,
    AddressInformationDto  AddressInformation,
    ContactInfomationDto  ContactInformation,
    string? LogoUrl,
    string CreatedBy) : IRequest<ResultReponse<Guid>>;
