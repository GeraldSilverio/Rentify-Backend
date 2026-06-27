using MediatR;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Emails.Commands.SendTemplateEmail
{
    public class SendTemplateEmailHandler : IRequestHandler<SendTemplateEmailCommand, ResultReponse<SendTemplateEmailResponse>>
    {
        private readonly IEmailService _emailService;

        public SendTemplateEmailHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task<ResultReponse<SendTemplateEmailResponse>> Handle(SendTemplateEmailCommand request, CancellationToken cancellationToken)
        {
            var result = await _emailService.SendEmailAsync(request, cancellationToken);

            return ResultReponse<SendTemplateEmailResponse>.Success(result);
        }
    }
}
