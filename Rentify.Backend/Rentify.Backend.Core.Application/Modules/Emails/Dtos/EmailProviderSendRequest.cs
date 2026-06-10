namespace Rentify.Backend.Core.Application.Modules.Emails.Dtos
{
    public record EmailProviderSendRequest(
        string ApiKey,
        string FromEmail,
        string FromName,
        string To,
        string Subject,
        string HtmlBody,
        string? TextBody);
}
