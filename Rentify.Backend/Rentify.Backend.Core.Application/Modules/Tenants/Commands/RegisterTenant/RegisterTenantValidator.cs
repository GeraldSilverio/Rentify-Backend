using FluentValidation;
using Rentify.Backend.Core.Application.Modules.Tenants.Commands.RegisterTenant;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.RegisterTenant;

public sealed class RegisterTenantValidator
    : AbstractValidator<RegisterTenantCommand>
{
    public RegisterTenantValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);

        RuleFor(x => x.RentCarName)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Phone)
            .NotEmpty();

        RuleFor(x => x.Country)
            .NotEmpty();

        RuleFor(x => x.PlanCode)
            .NotEmpty();
    }
}