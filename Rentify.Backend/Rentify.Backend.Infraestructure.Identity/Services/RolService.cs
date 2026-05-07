using Microsoft.AspNetCore.Identity;
using Rentify.Backend.Core.Application.Dtos.Roles;
using Rentify.Backend.Core.Application.Interfaces.Repositories;
using Rentify.Backend.Core.Application.Interfaces.Services;
using Rentify.Backend.Core.Application.Wrappers;
using Rentify.Backend.Infraestructure.Identity.Repositories.Interfaces;

namespace Rentify.Backend.Infraestructure.Identity.Services;

public class RolService(IRolRepository rolRepository) : IRolService
{
    public async Task<Result<List<RolDto>>> GetRolesAsync()
    {
        try
        {
            List<IdentityRole> identityRoles = await rolRepository.GetAllRolesAsync();

            if (identityRoles != null)
            {
                List<RolDto> roles = identityRoles.Select(x => new RolDto(x.Id, x.Name)).ToList();

                return Result<List<RolDto>>.Success(roles);
            }

            return Result<List<RolDto>>.Failure(Error.NotFound("No roles found."));
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<Result<RolDto>> GetRoleByIdAsync(string id)
    {
        try
        {
            IdentityRole identityRole = await rolRepository.FindRoleByIdAsync(id);

            if (identityRole != null)
            {
                return Result<RolDto>.Success(new RolDto(identityRole.Id, identityRole.Name));
            }

            return Result<RolDto>.Failure(Error.NotFound("Role not found."));
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<Result<RolDto>> AddRoleAsync(SaveRoleRequest request)
    {
        try
        {
            var role = new IdentityRole(request.Name);
            await rolRepository.AddAsync(role);
            return Result<RolDto>.Success(new RolDto(role.Id, role.Name));
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<Result<RolDto>> UpdateRoleAsync(string id, SaveRoleRequest request)
    {
        try
        {
            var role = await rolRepository.FindRoleByIdAsync(id);
            if (role == null) return null;

            role.Name = request.Name;
            await rolRepository.UpdateAsync(role);

            return Result<RolDto>.Success(new RolDto(role.Id, role.Name));
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<Result<string>> DeleteRoleAsync(string id)
    {
        try
        {
            var role = await rolRepository.FindRoleByIdAsync(id);
            if (role != null)
            {
                await rolRepository.DeleteAsync(role);
                return Result<string>.Success("Role deleted successfully.");
            }
            return Result<string>.Failure(Error.NotFound("Role not found."));
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}