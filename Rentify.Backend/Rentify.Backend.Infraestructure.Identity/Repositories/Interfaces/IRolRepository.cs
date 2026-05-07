using Microsoft.AspNetCore.Identity;

namespace Rentify.Backend.Infraestructure.Identity.Repositories.Interfaces;

public interface IRolRepository
{
    Task AddAsync(IdentityRole role);
    Task<IdentityRole> FindRoleByIdAsync(string roleId);
    Task<List<IdentityRole>> GetAllRolesAsync();
    Task UpdateAsync(IdentityRole role);
    Task DeleteAsync(IdentityRole role);
}