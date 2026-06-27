using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Tenants.Dtos;
using Rentify.Backend.Core.Domain.Entities.Core;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.UpdateTenantProfile;

public sealed class UpdateTenantProfileHandler
    : IRequestHandler<UpdateTenantProfileCommand, ResultReponse<TenantProfileResponse>>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly ITenantReadRepository _tenantReadRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateTenantProfileHandler> _logger;

    public UpdateTenantProfileHandler(
        ITenantRepository tenantRepository,
        ITenantReadRepository tenantReadRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateTenantProfileHandler> logger)
    {
        _tenantRepository = tenantRepository;
        _tenantReadRepository = tenantReadRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ResultReponse<TenantProfileResponse>> Handle(
        UpdateTenantProfileCommand request,
        CancellationToken cancellationToken)
    {
        Tenant tenant = await _tenantRepository.GetByIdAsync(request.TenantId, cancellationToken)
            ?? throw new ApiException("Tenant not found.", StatusCodes.Status404NotFound);

        tenant.Update(
            request.Name,
            request.LegalName,
            request.Rnc,
            tenant.BusinessModel,
            request.ModifiedBy);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Tenant profile updated. TenantId: {TenantId}, ModifiedBy: {ModifiedBy}",
            request.TenantId,
            request.ModifiedBy);

        TenantProfileResponse response = await _tenantReadRepository.GetProfileAsync(request.TenantId, cancellationToken)
            ?? throw new ApiException("Tenant not found.", StatusCodes.Status404NotFound);

        return ResultReponse<TenantProfileResponse>.Success(response);
    }
}
