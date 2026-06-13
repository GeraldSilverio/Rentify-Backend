using FluentValidation;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Emails.Commands.ConfigureTenantEmail
{
    public class ConfigureTenantEmailValidator : AbstractValidator<ConfigureTenantEmailCommand>
    {
        public ConfigureTenantEmailValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty().WithMessage("TenantId is required");

            RuleFor(x => x.Provider)
                .IsInEnum().WithMessage("Provider is not valid")
                .Must(x => x == EmailProviderType.Resend).WithMessage("Only Resend is currently implemented");

            RuleFor(x => x.ApiKey)
                .NotEmpty().WithMessage("ApiKey is required")
                .MaximumLength(1000).WithMessage("ApiKey must have 1000 characters or fewer");

            RuleFor(x => x.FromEmail)
                .NotEmpty().WithMessage("FromEmail is required")
                .EmailAddress().WithMessage("Please enter a valid email address.");

            RuleFor(x => x.FromName)
                .NotEmpty().WithMessage("FromName is required")
                .MaximumLength(150).WithMessage("FromName must have 150 characters or fewer");

            RuleFor(x => x.CreatedBy)
                .NotEmpty().WithMessage("CreatedBy is required");
        }
    }
}
