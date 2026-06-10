using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Emails.Commands.SendTemplateEmail
{
    public class SendTemplateEmailValidator : AbstractValidator<SendTemplateEmailCommand>
    {
        public SendTemplateEmailValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty().WithMessage("TenantId is required");

            RuleFor(x => x.TemplateCode)
                .NotEmpty().WithMessage("TemplateCode is required")
                .MaximumLength(100).WithMessage("TemplateCode must have 100 characters or fewer");

            RuleFor(x => x.To)
                .NotEmpty().WithMessage("To is required")
                .EmailAddress().WithMessage("Please enter a valid email address.");
        }
    }
}
