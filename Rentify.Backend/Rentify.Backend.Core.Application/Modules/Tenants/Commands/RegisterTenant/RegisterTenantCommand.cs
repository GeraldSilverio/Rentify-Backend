using MediatR;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.RegisterTenant;

public sealed record RegisterTenantCommand(
    string RentCarName,
    string CreatedBy
) : IRequest<ResultReponse<RegisterTenantResponse>>;