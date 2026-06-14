namespace Rentify.Backend.Core.Application.Modules.Emails.Dtos
{
    public record EmailTemplateResponse(
        Guid Id,
        string Code,
        string Name,
        string Subject,
        string HtmlBody,
        string? TextBody,
        bool IsActive);
}
