using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Users.Commands.CreateUser
{
    public record CreateUserCommand(
        string FullName,
        string UserName,
        string Email,
        string Password,
        string PhoneNumber,
        Guid TenantId,
        string CreatedBy,
        string Role) : IRequest<ResultReponse<CreateUserResponse>>;
}
