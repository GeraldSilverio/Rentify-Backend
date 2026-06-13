using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Common.Helpers;
using Rentify.Backend.Core.Application.Modules.Tenants.Commands.RegisterTenant;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Services;
using Rentify.Backend.Core.Application.Shared.Exceptions;
using Rentify.Backend.Core.Domain.Entities.Core;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Implementations.Services
{
    public class TenantService : ITenantService
    {
        private readonly ITenantRepository _tenantRepository;

        public TenantService(ITenantRepository tenantRepository)
        {
            _tenantRepository = tenantRepository;
        }

        public async Task<Guid> CreateTenantAsync(RegisterTenantCommand registerTenantCommand, CancellationToken cancellationToken)
        {
            try
            {
                var slug = SlugGenerator.Generate(registerTenantCommand.RentCarName);

                if (await _tenantRepository.SlugExistsAsync(slug, cancellationToken))
                {
                    slug += "-" + Guid.NewGuid()
                   .ToString()[..6];
                }
                
                var tenant = Tenant.Create(registerTenantCommand.RentCarName, slug, registerTenantCommand.CreatedBy);

                await _tenantRepository.AddAsync(
                    tenant,
                    cancellationToken);

                return tenant.Id;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, StatusCodes.Status500InternalServerError);
            }
        }
    }
}
