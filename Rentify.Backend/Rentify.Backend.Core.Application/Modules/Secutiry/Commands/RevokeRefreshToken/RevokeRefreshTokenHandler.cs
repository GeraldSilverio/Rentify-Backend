using MediatR;
using Rentify.Backend.Core.Application.Modules.Secutiry.Contracts.Services;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Secutiry.Commands.RevokeRefreshToken
{
    public class RevokeRefreshTokenHandler : IRequestHandler<RevokeRefreshTokenCommand, ResultReponse<bool>>
    {
        private readonly IAuthenticationService _authenticationService;

        public RevokeRefreshTokenHandler(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task<ResultReponse<bool>> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var result = await _authenticationService.RevokeRefreshTokenAsync(request);

            return ResultReponse<bool>.Success(result);
        }
    }
}
