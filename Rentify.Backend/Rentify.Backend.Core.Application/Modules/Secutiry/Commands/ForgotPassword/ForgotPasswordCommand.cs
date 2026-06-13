using MediatR;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Secutiry.Commands.ForgotPassword
{
    public record ForgotPasswordCommand(
        string Email,
        string ResetPasswordUrl) : IRequest<ResultReponse<ForgotPasswordResponse>>;
}
