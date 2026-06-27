using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Emails.Commands.SendTemplateEmail
{
    public record SendTemplateEmailCommand(
        Guid TenantId,
        string TemplateCode,
        string To,
        Dictionary<string, string>? Variables) : IRequest<ResultReponse<SendTemplateEmailResponse>>;
}
