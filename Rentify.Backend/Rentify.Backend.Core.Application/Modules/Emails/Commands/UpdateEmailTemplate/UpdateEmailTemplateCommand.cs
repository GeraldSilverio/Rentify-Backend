using MediatR;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Emails.Commands.UpdateEmailTemplate
{
    public record UpdateEmailTemplateCommand(
        Guid Id,
        string Name,
        string Subject,
        string HtmlBody,
        string? TextBody,
        string ModifiedBy) : IRequest<ResultReponse<Guid>>;
}
