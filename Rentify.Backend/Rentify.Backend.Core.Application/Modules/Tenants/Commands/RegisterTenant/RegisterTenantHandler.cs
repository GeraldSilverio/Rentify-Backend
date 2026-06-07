using MediatR;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Services;
using Rentify.Backend.Core.Application.Shared.Response;
using Rentify.Backend.Core.Application.Shared.UnitOfWork;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.RegisterTenant;

public sealed class RegisterTenantHandler : IRequestHandler<RegisterTenantCommand,ResultReponse<RegisterTenantResponse>>
{
    private readonly ITenantService _tenantService;
    private readonly IUnitOfWork _unitOfWork;
    
    public RegisterTenantHandler(ITenantService tenantService,IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _tenantService = tenantService;
    }

    public async Task<ResultReponse<RegisterTenantResponse>> Handle(
        RegisterTenantCommand request,
        CancellationToken cancellationToken)
    {
        Guid tenantId = await _tenantService.CreateTenantAsync(request,cancellationToken);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ResultReponse<RegisterTenantResponse>.Success(new RegisterTenantResponse(tenantId));
    }
}