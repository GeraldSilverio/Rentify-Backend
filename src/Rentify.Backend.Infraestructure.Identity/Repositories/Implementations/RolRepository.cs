using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Infraestructure.Identity.Repositories.Interfaces;

namespace Rentify.Backend.Infraestructure.Identity.Repositories.Implementations;

public class RolRepository(RoleManager<IdentityRole> roleManager) : IRolRepository
{
    public async Task AddAsync(IdentityRole role)
    {
        try
        {
            await roleManager.CreateAsync(role);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<IdentityRole> FindRoleByIdAsync(string roleId)
    {
        try
        {
            IdentityRole? rol = await roleManager.FindByIdAsync(roleId);
            
            return rol;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<List<IdentityRole>> GetAllRolesAsync()
    {
        try
        {
            return await roleManager.Roles.ToListAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task UpdateAsync(IdentityRole role)
    {
        try
        {
            await roleManager.UpdateAsync(role);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task DeleteAsync(IdentityRole role)
    {
        try
        {
            await roleManager.DeleteAsync(role);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}