using Rentify.Backend.Infraestructure.Identity.Entities;

namespace Rentify.Backend.Infraestructure.Identity.Contracts.Services
{
    public interface IJwtServices
    {
        Task<string> GenerateSecurityTokenAsync(ApplicationUser user);
    }
}