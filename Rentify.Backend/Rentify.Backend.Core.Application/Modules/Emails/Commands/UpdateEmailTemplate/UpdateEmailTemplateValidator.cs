using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Emails.Commands.UpdateEmailTemplate
{
    public class UpdateEmailTemplateValidator : AbstractValidator<UpdateEmailTemplateCommand>
    {
        public UpdateEmailTemplateValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(150).WithMessage("Name must have 150 characters or fewer");

            RuleFor(x => x.Subject)
                .NotEmpty().WithMessage("Subject is required")
                .MaximumLength(250).WithMessage("Subject must have 250 characters or fewer");

            RuleFor(x => x.HtmlBody)
                .NotEmpty().WithMessage("HtmlBody is required");

            RuleFor(x => x.ModifiedBy)
                .NotEmpty().WithMessage("ModifiedBy is required");
        }
    }
}
