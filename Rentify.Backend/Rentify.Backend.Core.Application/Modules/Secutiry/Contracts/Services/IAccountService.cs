using Rentify.Backend.Core.Application.Modules.Secutiry.Commands.Login;
using Rentify.Backend.Core.Application.Modules.Secutiry.Dtos.Response;
using Rentify.Backend.Core.Application.Modules.Users.Commands.CreateUser;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Secutiry.Contracts.Services;

public interface IAccountService
{
    Task<bool> ExistsByEmailAsync(
        string email);

    Task<bool> ExistByUserNameAsync(string userName);

    Task<Guid> CreateUserAsync(
        CreateUserCommand createUserCommand);

    Task AddToRoleAsync(
        Guid userId,
        string role);

    Task<TokenResponse> GenerateJwtAsync(
        Guid userId);
}