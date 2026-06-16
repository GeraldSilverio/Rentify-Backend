using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Emails.Commands.SendTemplateEmail;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Secutiry;
using Rentify.Backend.Core.Application.Modules.Secutiry.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Subscriptions.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Users.Commands.CreateUser;
using Rentify.Backend.Core.Application.Shared.Exceptions;
using Rentify.Backend.Core.Application.Shared.Helpers;
using Rentify.Backend.Core.Application.Shared.Response;
using Rentify.Backend.Core.Application.Shared.Security;
using Rentify.Backend.Core.Application.Shared.UnitOfWork;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.RegisterTenant;

public sealed class RegisterTenantHandler : IRequestHandler<RegisterTenantCommand, ResultReponse<RegisterTenantResponse>>
{
    private readonly ITenantService _tenantService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IAccountService _accountService;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterTenantHandler(
        ITenantService tenantService,
        ISubscriptionService subscriptionService,
        IAccountService accountService,
        IEmailService emailService,
        IUnitOfWork unitOfWork)
    {
        _tenantService = tenantService;
        _subscriptionService = subscriptionService;
        _accountService = accountService;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultReponse<RegisterTenantResponse>> Handle(
        RegisterTenantCommand request,
        CancellationToken cancellationToken)
    {
        if (await _accountService.ExistsByEmailAsync(request.OwnerEmail))
        {
            throw new ApiException("Owner email already exists", StatusCodes.Status400BadRequest);
        }

        var tenantId = await _tenantService.CreateTenantAsync(request, cancellationToken);

        var subscription = await _subscriptionService.RegisterSubscriptionAsync(tenantId, request, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var ownerUserId = await _accountService.CreateUserAsync(new CreateUserCommand(
            request.OwnerFullName,
            request.OwnerUserName,
            request.OwnerEmail,
            request.OwnerPassword,
            request.OwnerPhoneNumber,
            tenantId,
            request.CreatedBy,
            ApplicationRoles.Owner));

        var welcomeEmailSent = await TrySendOwnerWelcomeEmailAsync(
            request,
            tenantId,
            subscription,
            cancellationToken);

        return ResultReponse<RegisterTenantResponse>.Success(new RegisterTenantResponse(
            tenantId,
            subscription.Id,
            ownerUserId,
            subscription.ExpiresAt,
            subscription.TrialEndsAt,
            welcomeEmailSent
                ? "Successfully registered tenant. Welcome email sent."
                : "Successfully registered tenant. Welcome email could not be sent."));
    }

    private async Task<bool> TrySendOwnerWelcomeEmailAsync(
        RegisterTenantCommand request,
        Guid tenantId,
        Core.Domain.Entities.Core.Subscription subscription,
        CancellationToken cancellationToken)
    {
        try
        {
            await _emailService.SendEmailAsync(new SendTemplateEmailCommand(
                tenantId,
                EmailTemplateCodes.OwnerWelcome,
                request.OwnerEmail,
                new Dictionary<string, string>
                {
                    ["OwnerFullName"] = request.OwnerFullName,
                    ["OwnerEmail"] = request.OwnerEmail,
                    ["TenantName"] = request.RentCarName,
                    ["SubscriptionPlanName"] = FormatPlanName(request.SubscriptionPlanCode),
                    ["SubscriptionStatus"] = subscription.Status.ToString(),
                    ["SubscriptionStartDate"] = FormatDate(subscription.StartsAt),
                    ["SubscriptionExpiresAt"] = FormatDate(subscription.ExpiresAt),
                    ["DashboardUrl"] = ReadFromConfiguration.GetValueFromConfig("RENTIFY_DASHBOARD_URL"),
                    ["SupportEmail"] = ReadFromConfiguration.GetValueFromConfig("SUPPORT_EMAIL")
                }),
                cancellationToken);

            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            return false;
        }
    }

    private static string FormatPlanName(string subscriptionPlanCode)
    {
        var normalizedCode = subscriptionPlanCode.Trim().ToLowerInvariant();

        return string.IsNullOrWhiteSpace(normalizedCode)
            ? "Starter"
            : char.ToUpperInvariant(normalizedCode[0]) + normalizedCode[1..];
    }

    private static string FormatDate(DateTime date)
    {
        return date.ToString("yyyy-MM-dd");
    }
}
