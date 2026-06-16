using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Secutiry.Commands.ForgotPassword
{
    public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Please enter a valid email address.");

            RuleFor(x => x.ResetPasswordUrl)
                .NotEmpty().WithMessage("ResetPasswordUrl is required")
                .Must(x => Uri.TryCreate(x, UriKind.Absolute, out _))
                .WithMessage("ResetPasswordUrl must be a valid absolute URL.");
        }
    }
}
