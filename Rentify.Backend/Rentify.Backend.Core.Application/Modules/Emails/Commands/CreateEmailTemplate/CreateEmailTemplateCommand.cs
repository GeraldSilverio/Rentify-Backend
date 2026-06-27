using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Emails.Commands.CreateEmailTemplate
{
    public record CreateEmailTemplateCommand(
        Guid? TenantId,
        string Code,
        string Name,
        string Subject,
        string HtmlBody,
        string? TextBody,
        string CreatedBy) : IRequest<ResultReponse<Guid>>;
}
