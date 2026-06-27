using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Tenants.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Queries.GetTenantPaymentPolicy;

public sealed class GetTenantPaymentPolicyHandler
    : IRequestHandler<GetTenantPaymentPolicyQuery, ResultReponse<TenantPaymentPolicyResponse>>
{
    private readonly ITenantReadRepository _tenantReadRepository;

    public GetTenantPaymentPolicyHandler(ITenantReadRepository tenantReadRepository)
    {
        _tenantReadRepository = tenantReadRepository;
    }

    public async Task<ResultReponse<TenantPaymentPolicyResponse>> Handle(
        GetTenantPaymentPolicyQuery request,
        CancellationToken cancellationToken)
    {
        TenantPaymentPolicyResponse? paymentPolicy = await _tenantReadRepository
            .GetDefaultPaymentPolicyAsync(request.TenantId, cancellationToken);

        if (paymentPolicy is null)
            throw new ApiException("Default payment policy not found.", StatusCodes.Status404NotFound);

        return ResultReponse<TenantPaymentPolicyResponse>.Success(paymentPolicy);
    }
}
