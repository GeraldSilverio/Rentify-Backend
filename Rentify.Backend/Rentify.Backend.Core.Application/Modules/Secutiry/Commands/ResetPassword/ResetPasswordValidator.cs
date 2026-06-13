using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Secutiry.Commands.ResetPassword
{
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Please enter a valid email address.");

            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Token is required");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must have 8 characters")
                .Matches("[A-Z]").WithMessage("Password must have 1 capital letter")
                .Matches("[a-z]").WithMessage("Password must have 1 minuscule letter")
                .Matches("[0-9]").WithMessage("Password must have 1 number")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must have 1 special character");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("ConfirmPassword is required")
                .Equal(x => x.Password).WithMessage("ConfirmPassword must match Password");
        }
    }
}
