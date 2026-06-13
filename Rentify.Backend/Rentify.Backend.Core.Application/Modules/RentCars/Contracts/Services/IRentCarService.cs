using Rentify.Backend.Core.Application.Modules.RentCars.Commands.CreateRentCar;
using Rentify.Backend.Core.Application.Modules.RentCars.Commands.UpdateRentCar;

namespace Rentify.Backend.Core.Application.Modules.RentCars.Contracts.Services;

public interface IRentCarService
{
    Task<Guid> CreateRentCarAsync(CreateRentCarCommand command,CancellationToken cancellationToken = default);
    Task<Guid> UpdateRentCarAsync(UpdateRentCarCommand command, CancellationToken cancellationToken = default);
}
