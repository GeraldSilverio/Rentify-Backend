namespace Rentify.Backend.Core.Application.Modules.Emails.Commands.UpdateEmailTemplate
{
    public record UpdateEmailTemplateRequest(
        string Name,
        string Subject,
        string HtmlBody,
        string? TextBody,
        string ModifiedBy);
}
