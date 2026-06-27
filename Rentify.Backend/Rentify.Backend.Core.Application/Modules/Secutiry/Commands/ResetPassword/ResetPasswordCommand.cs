using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Secutiry.Commands.ResetPassword
{
    public record ResetPasswordCommand(
        string Email,
        string Token,
        string Password,
        string ConfirmPassword) : IRequest<ResultReponse<bool>>;
}
