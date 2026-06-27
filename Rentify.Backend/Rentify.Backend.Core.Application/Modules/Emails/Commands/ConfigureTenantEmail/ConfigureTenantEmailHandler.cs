using MediatR;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Emails.Commands.ConfigureTenantEmail
{
    public class ConfigureTenantEmailHandler : IRequestHandler<ConfigureTenantEmailCommand, ResultReponse<Guid>>
    {
        private readonly IEmailService _emailService;

        public ConfigureTenantEmailHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task<ResultReponse<Guid>> Handle(ConfigureTenantEmailCommand request, CancellationToken cancellationToken)
        {
            var result = await _emailService.ConfigureTenantEmailAsync(request, cancellationToken);

            return ResultReponse<Guid>.Success(result);
        }
    }
}
