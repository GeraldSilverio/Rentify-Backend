using Rentify.Backend.Core.Application.Modules.RentCars.Commands.UpdateRentCar;
using Rentify.Backend.Core.Application.Modules.RentCars.Commands.UploadRentCarLogo;
using Rentify.Backend.Core.Application.Modules.RentCars.Dtos;

namespace Rentify.Backend.Core.Application.Modules.RentCars.Contracts.Services;

public interface IRentCarService
{
    Task<Guid> CreateRentCarAsync(CreateRentCarDto createRentCarDto,CancellationToken cancellationToken = default);
    Task<Guid> UpdateRentCarAsync(UpdateRentCarCommand command, CancellationToken cancellationToken = default);
    Task<string> UploadLogoAsync(UploadRentCarLogoCommand command, CancellationToken cancellationToken = default);
}
