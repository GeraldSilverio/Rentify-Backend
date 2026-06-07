namespace Rentify.Backend.Core.Application.Modules.Users.Commands.CreateUser
{
    public record CreateUserResponse(
        Guid UserId,
        Guid TenantId);
}
