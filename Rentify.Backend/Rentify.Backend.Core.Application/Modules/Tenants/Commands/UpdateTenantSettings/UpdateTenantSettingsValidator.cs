using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.UpdateTenantSettings;

public sealed class UpdateTenantSettingsValidator : AbstractValidator<UpdateTenantSettingsCommand>
{
    public UpdateTenantSettingsValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.CurrencyCode).NotEmpty().MaximumLength(10);
        RuleFor(x => x.TimeZone).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ModifiedBy).NotEmpty();
    }
}
