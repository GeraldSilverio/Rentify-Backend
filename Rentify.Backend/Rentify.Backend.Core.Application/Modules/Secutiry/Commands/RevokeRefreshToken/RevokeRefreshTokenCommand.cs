using MediatR;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Secutiry.Commands.RevokeRefreshToken
{
    public record RevokeRefreshTokenCommand(string RefreshToken) : IRequest<ResultReponse<bool>>;
}
