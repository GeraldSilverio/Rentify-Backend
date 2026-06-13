namespace Rentify.Backend.Core.Application.Modules.Secutiry.Commands.ForgotPassword
{
    public record ForgotPasswordResponse(string Email, string ResetToken);
}
