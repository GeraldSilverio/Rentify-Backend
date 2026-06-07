using MediatR;
using Rentify.Backend.Core.Application.Modules.Secutiry.Contracts.Services;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Users.Commands.CreateUser
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, ResultReponse<CreateUserResponse>>
    {
        private readonly IAccountService _identityService;

        public CreateUserHandler(IAccountService identityService)
        {
            _identityService = identityService;
        }

        public async Task<ResultReponse<CreateUserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var userId = await _identityService.CreateUserAsync(request);
            return ResultReponse<CreateUserResponse>.Success(new CreateUserResponse(userId,request.TenantId));
        }
    }
}
