using MediatR;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Emails.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Emails.Queries.ListTenantEmailConfigurations
{
    public class ListTenantEmailConfigurationsHandler : IRequestHandler<ListTenantEmailConfigurationsQuery, ResultReponse<IReadOnlyList<TenantEmailConfigurationResponse>>>
    {
        private readonly ITenantEmailConfigurationRepository _tenantEmailConfigurationRepository;

        public ListTenantEmailConfigurationsHandler(ITenantEmailConfigurationRepository tenantEmailConfigurationRepository)
        {
            _tenantEmailConfigurationRepository = tenantEmailConfigurationRepository;
        }

        public async Task<ResultReponse<IReadOnlyList<TenantEmailConfigurationResponse>>> Handle(ListTenantEmailConfigurationsQuery request, CancellationToken cancellationToken)
        {
            var configurations = await _tenantEmailConfigurationRepository.ListByTenantAsync(request.TenantId, cancellationToken);

            var response = configurations
                .Select(x => new TenantEmailConfigurationResponse(
                    x.Id,
                    x.TenantId,
                    x.Provider,
                    x.FromEmail,
                    x.FromName,
                    x.IsDefault,
                    x.IsActive,
                    MaskApiKey(x.ApiKey)))
                .ToList();

            return ResultReponse<IReadOnlyList<TenantEmailConfigurationResponse>>.Success(response);
        }

        private static string MaskApiKey(string apiKey)
        {
            if (apiKey.Length <= 8)
            {
                return "********";
            }

            return $"{apiKey[..4]}********{apiKey[^4..]}";
        }
    }
}
