using MediatR;
using Rentify.Backend.Core.Application.Shared.Dtos.Information;
using Rentify.Backend.Core.Application.Shared.Dtos.User;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.RegisterTenant;

public sealed record RegisterTenantCommand(
    string RentCarName,
    UserInfomationDto UserInfomation,
    AddressInformationDto AddressInformation,
    ContactInformationDto ContactInformation,
    string CreatedBy,
    string SubscriptionPlanCode = "STARTER",
    int TrialDays = 14
) : IRequest<ResultReponse<RegisterTenantResponse>>;
