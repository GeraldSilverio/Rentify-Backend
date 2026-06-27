using MediatR;
using Rentify.Backend.Core.Application.Modules.Secutiry.Dtos.Response;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Secutiry.Commands.RefreshToken
{
    public record RefreshTokenCommand(string RefreshToken) : IRequest<ResultReponse<TokenResponse>>;
}
