using Rentify.Backend.Core.Application.Modules.RentCars.Commands.CreateRentCar;
using Rentify.Backend.Core.Application.Modules.Tenants.Commands.RegisterTenant;

namespace Rentify.Backend.Core.Application.Modules.RentCars.Contracts.Services;

public interface IRentCarService
{
    Task<Guid> CreateRentCarAsync(CreateRentCarCommand command,CancellationToken cancellationToken = default);
}