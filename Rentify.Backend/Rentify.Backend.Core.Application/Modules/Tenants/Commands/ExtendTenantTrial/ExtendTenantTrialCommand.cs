using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Tenants.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.ExtendTenantTrial;

public sealed record ExtendTenantTrialCommand(
    Guid TenantId,
    int DaysToAdd,
    string ModifiedBy) : IRequest<ResultReponse<TenantSubscriptionResponse>>;
