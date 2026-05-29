using Rentify.Backend.Core.Application.Modules.RentCars.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.RentCars.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Tenants.Commands.RegisterTenant;
using Rentify.Backend.Core.Domain.Entities;

namespace Rentify.Backend.Core.Application.Modules.RentCars.Implementations.Services;

public sealed class RentCarService : IRentCarService
{
    private readonly IRentCarRepository _rentCarRepository;
    
    public RentCarService(IRentCarRepository rentCarRepository)
    {
        _rentCarRepository = rentCarRepository;
    }
    
    public async Task CreateRentCarAsync(RegisterTenantCommand command, Guid tenantId, CancellationToken cancellationToken = default)
    {
        RentCar rentCar = RentCar.Create(
            command.RentCarName,
            command.Description,
            command.Phone,
            command.Email,
            command.WhatsApp,
            command.Street,
            command.City,
            command.Country,
            command.Email);
        
        rentCar.SetTenantId(tenantId);

        await _rentCarRepository.AddAsync(
            rentCar,
            cancellationToken);
    }
}