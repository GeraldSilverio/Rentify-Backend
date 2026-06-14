using MediatR;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Emails.Dtos;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Emails.Queries.ListEmailTemplates
{
    public class ListEmailTemplatesHandler : IRequestHandler<ListEmailTemplatesQuery, ResultReponse<IReadOnlyList<EmailTemplateResponse>>>
    {
        private readonly ISystemEmailTemplateRepository _emailTemplateRepository;

        public ListEmailTemplatesHandler(ISystemEmailTemplateRepository emailTemplateRepository)
        {
            _emailTemplateRepository = emailTemplateRepository;
        }

        public async Task<ResultReponse<IReadOnlyList<EmailTemplateResponse>>> Handle(ListEmailTemplatesQuery request, CancellationToken cancellationToken)
        {
            var templates = await _emailTemplateRepository.ListAsync(request.TenantId, cancellationToken);

            var response = templates
                .Select(x => new EmailTemplateResponse(x.Id, x.Code, x.Name, x.Subject, x.HtmlBody, x.TextBody, x.IsActive))
                .ToList();

            return ResultReponse<IReadOnlyList<EmailTemplateResponse>>.Success(response);
        }
    }
}
