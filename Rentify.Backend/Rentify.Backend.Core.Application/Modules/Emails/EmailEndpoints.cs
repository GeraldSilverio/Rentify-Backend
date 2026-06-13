using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rentify.Backend.Core.Application.Modules.Emails.Commands.ConfigureTenantEmail;
using Rentify.Backend.Core.Application.Modules.Emails.Commands.CreateEmailTemplate;
using Rentify.Backend.Core.Application.Modules.Emails.Commands.SendTemplateEmail;
using Rentify.Backend.Core.Application.Modules.Emails.Commands.UpdateEmailTemplate;
using Rentify.Backend.Core.Application.Modules.Emails.Queries.GetEmailTemplate;
using Rentify.Backend.Core.Application.Modules.Emails.Queries.ListEmailTemplates;
using Rentify.Backend.Core.Application.Modules.Emails.Queries.ListTenantEmailConfigurations;

namespace Rentify.Backend.Core.Application.Modules.Emails
{
    public static class EmailEndpoints
    {
        public static IEndpointRouteBuilder MapEmailEndpoints(this IEndpointRouteBuilder app)
        {
            var emails = app.MapGroup("/api/v1/emails")
                .WithTags("Emails");

            emails.MapCreateEmailTemplate();
            emails.MapListEmailTemplates();
            emails.MapGetEmailTemplate();
            emails.MapUpdateEmailTemplate();
            emails.MapConfigureTenantEmail();
            emails.MapListTenantEmailConfigurations();
            emails.MapSendTemplateEmail();

            return app;
        }
    }
}
