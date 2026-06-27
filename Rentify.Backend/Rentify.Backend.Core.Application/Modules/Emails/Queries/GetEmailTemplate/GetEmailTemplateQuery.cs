using MediatR;
using Rentify.Backend.Core.Application.Modules.Emails.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Emails.Queries.GetEmailTemplate
{
    public record GetEmailTemplateQuery(Guid Id) : IRequest<ResultReponse<EmailTemplateResponse>>;
}
