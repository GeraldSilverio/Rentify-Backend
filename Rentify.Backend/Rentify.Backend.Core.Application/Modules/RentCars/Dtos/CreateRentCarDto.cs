using Rentify.Backend.Core.Application.Shared.Dtos.Information;

namespace Rentify.Backend.Core.Application.Modules.RentCars.Dtos
{
    public sealed record CreateRentCarDto(
        string Name,
        string Description,
        Guid TenantId,
        ContactInformationDto ContactInformation,
        AddressInformationDto AddressInformation,
        string CreatedBy);
}
