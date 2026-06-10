using MediatR;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Services;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Emails.Commands.CreateEmailTemplate
{
    public class CreateEmailTemplateHandler : IRequestHandler<CreateEmailTemplateCommand, ResultReponse<Guid>>
    {
        private readonly IEmailService _emailService;

        public CreateEmailTemplateHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task<ResultReponse<Guid>> Handle(CreateEmailTemplateCommand request, CancellationToken cancellationToken)
        {
            var result = await _emailService.CreateEmailTemplateAsync(request, cancellationToken);

            return ResultReponse<Guid>.Success(result);
        }
    }
}
