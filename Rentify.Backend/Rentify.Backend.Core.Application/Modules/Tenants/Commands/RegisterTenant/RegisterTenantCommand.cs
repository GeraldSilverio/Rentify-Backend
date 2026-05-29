using MediatR;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.RegisterTenant;

public sealed record RegisterTenantCommand(
    string FullName,
    string Email,
    string Password,
    string RentCarName,
    string Description,
    string Phone,
    string WhatsApp,
    string Street,
    string City,
    string Country,
    string PlanCode,
    string Role,
    int DaysSubscriptions
) : IRequest<ResultReponse<RegisterTenantResponse>>;