using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Core.Application.Modules.Secutiry.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Secutiry.Dtos.Response;
using Rentify.Backend.Core.Application.Modules.Tenants.Commands.RegisterTenant;
using Rentify.Backend.Core.Application.Shared.Exceptions;
using Rentify.Backend.Core.Application.Shared.Response;
using Rentify.Backend.Infraestructure.Identity.Contracts.Services;
using Rentify.Backend.Infraestructure.Identity.Entities;

namespace Rentify.Backend.Infrastructure.Identity.Services;

public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtServices _jwtProvider;
    private readonly RoleManager<IdentityRole> _roleManager;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        IJwtServices jwtProvider, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _jwtProvider = jwtProvider;
        _roleManager = roleManager;
    }

    public async Task<bool> ExistsByEmailAsync(
        string email)
    {
        return await _userManager.Users
            .AnyAsync(x => x.Email == email);
    }

    public async Task<ResultReponse<Guid>> CreateUserAsync(
        RegisterTenantCommand request,Guid tenantId)
    {
        if (await ExistsByEmailAsync(request.Email)) throw new ApiException("Email already exists", StatusCodes.Status400BadRequest);
        
        if(await _roleManager.FindByNameAsync(request.Role) == null ) throw new ApiException($"Role{request.Role} does not exist", StatusCodes.Status400BadRequest);
        
        var user = new ApplicationUser  
        {
            Id = Guid.NewGuid().ToString(),
            FullName = request.FullName,
            Email = request.Email,
            UserName = request.Email,
            TenantId = tenantId,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(
            user,
            request.Password);

        if (!result.Succeeded)
        {
            throw new ApiException(string.Join(", ", result.Errors.Select(x => x.Description)), StatusCodes.Status400BadRequest);
        }
        
        await _userManager.AddToRoleAsync(user,request.Role);
        
        

        return ResultReponse<Guid>.Success(Guid.Parse(user.Id));
    }

    public async Task AddToRoleAsync(
        Guid userId,
        string role)
    {
        var user = await _userManager.FindByIdAsync(
            userId.ToString());

        if (user is null)
            return;

        await _userManager.AddToRoleAsync(
            user,
            role);
    }

    public async Task<TokenResponse> GenerateJwtAsync(
        Guid userId)
    {
        var user = await _userManager.FindByIdAsync(
            userId.ToString());

        var token = await _jwtProvider.GenerateSecurityTokenAsync(user!);

        return new TokenResponse(token,DateTime.UtcNow.AddMinutes(60));
    }
}