using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Emails.Commands.CreateEmailTemplate
{
    public class CreateEmailTemplateValidator : AbstractValidator<CreateEmailTemplateCommand>
    {
        public CreateEmailTemplateValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required")
                .MaximumLength(100).WithMessage("Code must have 100 characters or fewer");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(150).WithMessage("Name must have 150 characters or fewer");

            RuleFor(x => x.Subject)
                .NotEmpty().WithMessage("Subject is required")
                .MaximumLength(250).WithMessage("Subject must have 250 characters or fewer");

            RuleFor(x => x.HtmlBody)
                .NotEmpty().WithMessage("HtmlBody is required");

            RuleFor(x => x.CreatedBy)
                .NotEmpty().WithMessage("CreatedBy is required");
        }
    }
}
