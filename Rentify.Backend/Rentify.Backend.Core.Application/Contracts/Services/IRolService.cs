using Rentify.Backend.Core.Application.Dtos.Roles;
using Rentify.Backend.Core.Application.Wrappers;

namespace Rentify.Backend.Core.Application.Interfaces.Services;

public interface IRolService
{
    Task<Result<List<RolDto>>> GetRolesAsync();

    Task<Result<RolDto>> GetRoleByIdAsync(string id);
    Task<Result<RolDto>> AddRoleAsync(SaveRoleRequest request);
    Task<Result<RolDto>> UpdateRoleAsync(string id, SaveRoleRequest request);
    Task<Result<string>> DeleteRoleAsync(string id);
}