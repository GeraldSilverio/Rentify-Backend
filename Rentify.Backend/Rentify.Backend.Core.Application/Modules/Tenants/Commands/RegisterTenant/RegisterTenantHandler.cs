using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Services;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.RegisterTenant;

public sealed class RegisterTenantHandler(ITenantService tenantService) : IRequestHandler<RegisterTenantCommand, ResultReponse<RegisterTenantResponse>>
{
    public async Task<ResultReponse<RegisterTenantResponse>> Handle(
        RegisterTenantCommand request,
        CancellationToken cancellationToken)
    {

        RegisterTenantResponse tenantResponse = await tenantService.CreateTenantAsync(request, cancellationToken);

        return ResultReponse<RegisterTenantResponse>.Success(tenantResponse);
    }
}
