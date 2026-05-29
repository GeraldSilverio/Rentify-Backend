using Rentify.Backend.Core.Application.Modules.Tenants.Commands.RegisterTenant;

namespace Rentify.Backend.Core.Application.Modules.RentCars.Contracts.Services;

public interface IRentCarService
{
    Task CreateRentCarAsync(RegisterTenantCommand command,Guid tenantId,CancellationToken cancellationToken = default);
}