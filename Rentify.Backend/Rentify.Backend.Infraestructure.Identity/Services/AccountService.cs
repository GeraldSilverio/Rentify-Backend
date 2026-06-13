using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Core.Application.Modules.Secutiry.Commands.Login;
using Rentify.Backend.Core.Application.Modules.Secutiry.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Secutiry.Dtos.Response;
using Rentify.Backend.Core.Application.Modules.Users.Commands.CreateUser;
using Rentify.Backend.Core.Application.Shared.Exceptions;
using Rentify.Backend.Infraestructure.Identity.Contracts.Services;
using Rentify.Backend.Infraestructure.Identity.Entities;

namespace Rentify.Backend.Infraestructure.Identity.Services;

public sealed class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtServices _jwtProvider;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountService(
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
        return await _userManager.FindByEmailAsync(email) != null;
    }

    public async Task<Guid> CreateUserAsync(CreateUserCommand createUserCommand)
    {
        if (await ExistsByEmailAsync(createUserCommand.Email)) throw new ApiException("Email already exists", StatusCodes.Status400BadRequest);

        if (await ExistByUserNameAsync(createUserCommand.UserName)) throw new ApiException("UserName already exists", StatusCodes.Status400BadRequest);
        
        if (await _roleManager.FindByNameAsync(createUserCommand.Role) == null ) throw new ApiException($"Role {createUserCommand.Role} does not exist", StatusCodes.Status400BadRequest);
        
        var user = new ApplicationUser  
        {
            Id = Guid.NewGuid().ToString(),
            FullName = createUserCommand.FullName,
            Email = createUserCommand.Email,
            UserName = createUserCommand.UserName,
            PhoneNumber = createUserCommand.PhoneNumber,
            TenantId = createUserCommand.TenantId,
            EmailConfirmed = true,
            IsActive = true,
            CreatedBy = createUserCommand.CreatedBy,
            ModifiedBy = createUserCommand.CreatedBy,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(
            user,
            createUserCommand.Password);

        if (!result.Succeeded)
        {
            throw new ApiException(string.Join(", ", result.Errors.Select(x => x.Description)), StatusCodes.Status400BadRequest);
        }
        
        await _userManager.AddToRoleAsync(user,createUserCommand.Role);

        return Guid.Parse(user.Id);
    }

    private async Task<bool> ExistByUserNameAsync(string userName)
    {
        ApplicationUser? user = await _userManager.FindByNameAsync(userName);

        return user != null;
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
