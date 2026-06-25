using MediatR;
using Rentify.Backend.Core.Application.Shared.Dtos.Information;
using Rentify.Backend.Core.Application.Shared.Dtos.User;
using Rentify.Backend.Core.Application.Shared.Response;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.RegisterTenant;

public sealed record RegisterTenantCommand(
    string Name,
    string LegalName,
    string Rnc,
    BusinessModel BusinessModel,
    UserInfomationDto UserInformation,
    AddressInformationDto AddressInformation,
    ContactInformationDto ContactInformation,
    string CreatedBy,
    string SubscriptionPlanCode = "STARTER",
    int TrialDays = 14
) : IRequest<ResultReponse<RegisterTenantResponse>>;
