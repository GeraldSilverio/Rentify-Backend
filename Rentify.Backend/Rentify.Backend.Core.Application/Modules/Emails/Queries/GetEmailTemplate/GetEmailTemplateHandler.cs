using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Emails.Dtos;
using Rentify.Backend.Core.Application.Shared.Exceptions;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Emails.Queries.GetEmailTemplate
{
    public class GetEmailTemplateHandler : IRequestHandler<GetEmailTemplateQuery, ResultReponse<EmailTemplateResponse>>
    {
        private readonly IEmailTemplateRepository _emailTemplateRepository;

        public GetEmailTemplateHandler(IEmailTemplateRepository emailTemplateRepository)
        {
            _emailTemplateRepository = emailTemplateRepository;
        }

        public async Task<ResultReponse<EmailTemplateResponse>> Handle(GetEmailTemplateQuery request, CancellationToken cancellationToken)
        {
            var template = await _emailTemplateRepository.GetByIdAsync(request.Id, cancellationToken);

            if (template == null)
            {
                throw new ApiException("Email template not found", StatusCodes.Status404NotFound);
            }

            var response = new EmailTemplateResponse(
                template.Id,
                template.TenantId,
                template.Code,
                template.Name,
                template.Subject,
                template.HtmlBody,
                template.TextBody,
                template.IsActive);

            return ResultReponse<EmailTemplateResponse>.Success(response);
        }
    }
}
