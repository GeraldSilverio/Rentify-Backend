using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Tenants.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Queries.GetTenantPaymentPolicy;

public sealed record GetTenantPaymentPolicyQuery(Guid TenantId) : IRequest<ResultReponse<TenantPaymentPolicyResponse>>;
