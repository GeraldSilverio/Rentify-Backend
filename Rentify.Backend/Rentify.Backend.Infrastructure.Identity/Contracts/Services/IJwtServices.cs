using Rentify.Backend.Infrastructure.Identity.Entities;

namespace Rentify.Backend.Infrastructure.Identity.Contracts.Services
{
    public interface IJwtServices
    {
        Task<string> GenerateSecurityTokenAsync(ApplicationUser user);
    }
}