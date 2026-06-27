using MediatR;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Emails.Commands.UpdateEmailTemplate
{
    public class UpdateEmailTemplateHandler : IRequestHandler<UpdateEmailTemplateCommand, ResultReponse<Guid>>
    {
        private readonly IEmailService _emailService;

        public UpdateEmailTemplateHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task<ResultReponse<Guid>> Handle(UpdateEmailTemplateCommand request, CancellationToken cancellationToken)
        {
            var result = await _emailService.UpdateEmailTemplateAsync(request.Id, request, cancellationToken);

            return ResultReponse<Guid>.Success(result);
        }
    }
}
