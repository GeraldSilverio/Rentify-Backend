using Rentify.Backend.Core.Application.Modules.Emails.Commands.SendTemplateEmail;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Secutiry;
using Rentify.Backend.Core.Application.Modules.Shared.Constants;
using Rentify.Backend.Core.Application.Modules.Shared.Contracts;
using Rentify.Backend.Core.Application.Modules.Shared.Helpers;
using Rentify.Backend.Core.Application.Modules.Tenants.Events;
using Rentify.Backend.Core.Domain.Enums;
using System.Text.Json;

namespace Rentify.Backend.Infraestructure.Shared.Services.OutBox
{
    public sealed class TenantRegisteredOutboxHandler : IOutboxMessageHandler
    {
        private readonly IEmailService _emailService;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public string Type => OutboxMessageTypes.TenantRegistered;

        public TenantRegisteredOutboxHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task HandleAsync(
            string payload,
            CancellationToken cancellationToken = default)
        {
            TenantRegisteredOutboxPayload? data =
                JsonSerializer.Deserialize<TenantRegisteredOutboxPayload>(payload, JsonOptions);

            if (data is null)
                throw new InvalidOperationException("Invalid TenantRegistered payload.");

            await _emailService.SendEmailAsync(new SendTemplateEmailCommand(
                data.TenantId,
                EmailTemplateCodes.OwnerWelcome,
                data.OwnerEmail,
                new Dictionary<string, string>
                {
                    ["OwnerFullName"] = data.OwnerFullName,
                    ["OwnerEmail"] = data.OwnerEmail,
                    ["TenantName"] = data.TenantName,
                    ["BusinessModel"] = FormatBusinessModel(data.BusinessModel),
                    ["SubscriptionPlanName"] = FormatPlanName(data.SubscriptionPlanCode),
                    ["SubscriptionStatus"] = data.SubscriptionStatus,
                    ["SubscriptionStartDate"] = FormatDate(data.SubscriptionStartsAt),
                    ["SubscriptionExpiresAt"] = FormatDate(data.SubscriptionExpiresAt),
                    ["TrialEndsAt"] = data.TrialEndsAt.HasValue ? FormatDate(data.TrialEndsAt.Value) : string.Empty,
                    ["DashboardUrl"] = ReadFromConfiguration.GetValueFromConfig("RENTIFY_DASHBOARD_URL"),
                    ["SupportEmail"] = ReadFromConfiguration.GetValueFromConfig("SUPPORT_EMAIL")
                }),
                cancellationToken);
        }

        private static string FormatPlanName(string subscriptionPlanCode)
        {
            if (string.IsNullOrWhiteSpace(subscriptionPlanCode))
                return "Starter";

            string normalizedCode = subscriptionPlanCode.Trim().ToLowerInvariant();

            return char.ToUpperInvariant(normalizedCode[0]) + normalizedCode[1..];
        }

        private static string FormatDate(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        private static string FormatBusinessModel(BusinessModel businessModel)
        {
            return businessModel switch
            {
                BusinessModel.TraditionalRentCar => "Rent car tradicional",
                BusinessModel.DriverFleetRental => "Renta de vehículos para choferes",
                BusinessModel.Mixed => "Modelo mixto",
                _ => "Rentify"
            };
        }
    }
}