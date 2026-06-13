using MediatR;
using Rentify.Backend.Core.Application.Modules.Emails.Dtos;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Emails.Queries.ListTenantEmailConfigurations
{
    public record ListTenantEmailConfigurationsQuery(Guid TenantId) : IRequest<ResultReponse<IReadOnlyList<TenantEmailConfigurationResponse>>>;
}
