using MediatR;
using Rentify.Backend.Core.Application.Modules.Secutiry.Contracts.Services;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Secutiry.Commands.ForgotPassword
{
    public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, ResultReponse<ForgotPasswordResponse>>
    {
        private readonly IAuthenticationService _authenticationService;

        public ForgotPasswordHandler(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task<ResultReponse<ForgotPasswordResponse>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var result = await _authenticationService.ForgotPasswordAsync(request);

            return ResultReponse<ForgotPasswordResponse>.Success(result);
        }
    }
}
