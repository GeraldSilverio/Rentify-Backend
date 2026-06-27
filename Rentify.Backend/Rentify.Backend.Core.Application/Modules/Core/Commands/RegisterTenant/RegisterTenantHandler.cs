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
    //private async Task<bool> TrySendOwnerWelcomeEmailAsync(
    //    RegisterTenantCommand request,
    //    Guid tenantId,
    //    Core.Domain.Entities.Core.Subscription subscription,
    //    CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        await _emailService.SendEmailAsync(new SendTemplateEmailCommand(
    //            tenantId,
    //            EmailTemplateCodes.OwnerWelcome,
    //            request.UserInfomation.ContactInformation.Email,
    //            new Dictionary<string, string>
    //            {
    //                ["OwnerFullName"] = request.UserInfomation.FullName,
    //                ["OwnerEmail"] = request.UserInfomation.ContactInformation.Email,
    //                ["TenantName"] = request.LegalName,
    //                ["SubscriptionPlanName"] = FormatPlanName(request.SubscriptionPlanCode),
    //                ["SubscriptionStatus"] = subscription.Status.ToString(),
    //                ["SubscriptionStartDate"] = FormatDate(subscription.StartsAt),
    //                ["SubscriptionExpiresAt"] = FormatDate(subscription.ExpiresAt),
    //                ["DashboardUrl"] = ReadFromConfiguration.GetValueFromConfig("RENTIFY_DASHBOARD_URL"),
    //                ["SupportEmail"] = ReadFromConfiguration.GetValueFromConfig("SUPPORT_EMAIL")
    //            }),
    //            cancellationToken);

    //        return true;
    //    }
    //    catch (OperationCanceledException)
    //    {
    //        throw;
    //    }
    //    catch
    //    {
    //        return false;
    //    }
    //}

    //private static string FormatPlanName(string subscriptionPlanCode)
    //{
    //    var normalizedCode = subscriptionPlanCode.Trim().ToLowerInvariant();

    //    return string.IsNullOrWhiteSpace(normalizedCode)
    //        ? "Starter"
    //        : char.ToUpperInvariant(normalizedCode[0]) + normalizedCode[1..];
    //}

    //private static string FormatDate(DateTime date)
    //{
    //    return date.ToString("yyyy-MM-dd");
    //}
}
