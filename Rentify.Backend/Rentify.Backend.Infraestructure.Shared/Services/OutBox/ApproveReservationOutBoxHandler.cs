using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Rentify.Backend.Core.Application.Modules.Emails.Commands.SendTemplateEmail;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Reservations.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Reservations.Dtos;
using Rentify.Backend.Core.Application.Modules.Reservations.Events;
using Rentify.Backend.Core.Application.Modules.Secutiry;
using Rentify.Backend.Core.Application.Modules.Shared.Constants;
using Rentify.Backend.Core.Application.Modules.Shared.Contracts;
using Rentify.Backend.Core.Application.Modules.Shared.Helpers;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Infraestructure.Shared.Services.OutBox;

internal sealed class ApproveReservationOutBoxHandler : IOutboxMessageHandler
{
    private static readonly CultureInfo DominicanCulture = CultureInfo.GetCultureInfo("es-DO");
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly IEmailService _emailService;
    private readonly IReservationRepository _reservationRepository;
    private readonly ILogger<ApproveReservationOutBoxHandler> _logger;

    public ApproveReservationOutBoxHandler(
        IEmailService emailService,
        IReservationRepository reservationRepository,
        ILogger<ApproveReservationOutBoxHandler> logger)
    {
        _emailService = emailService;
        _reservationRepository = reservationRepository;
        _logger = logger;
    }

    public string Type => OutboxMessageTypes.ReservationApproved;

    public async Task HandleAsync(string payload, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(payload))
            throw new InvalidOperationException("ReservationApproved payload is required.");

        ReservationApprovedOutboxPayload? data = JsonSerializer.Deserialize<ReservationApprovedOutboxPayload>(payload, JsonOptions);

        if (data is null || data.TenantId == Guid.Empty || data.ReservationId == Guid.Empty)
            throw new InvalidOperationException("ReservationApproved payload contains invalid identifiers.");

        _logger.LogInformation(
            "Processing reservation-approved email for reservation {ReservationId} in tenant {TenantId}",
            data.ReservationId,
            data.TenantId);

        ReservationApprovedEmailData emailData = await _reservationRepository.GetApprovedEmailDataAsync(
            data.TenantId,
            data.ReservationId,
            cancellationToken)
            ?? throw new InvalidOperationException("Approved reservation was not found while processing its notification.");

        if (string.IsNullOrWhiteSpace(emailData.CustomerEmail))
        {
            _logger.LogWarning(
                "Reservation-approved email was skipped because customer email is missing for reservation {ReservationId} in tenant {TenantId}",
                data.ReservationId,
                data.TenantId);
            return;
        }

        await _emailService.SendEmailAsync(new SendTemplateEmailCommand(
            data.TenantId,
            EmailTemplateCodes.ReservationApproved,
            emailData.CustomerEmail,
            BuildTemplateVariables(emailData)), cancellationToken);

        _logger.LogInformation(
            "Reservation-approved email processed for reservation {ReservationId} in tenant {TenantId}",
            data.ReservationId,
            data.TenantId);
    }

    private static Dictionary<string, string> BuildTemplateVariables(ReservationApprovedEmailData data)
    {
        string rentalType = TranslateRentalType(data.RentalType);

        return new Dictionary<string, string>
        {
            ["CustomerFirstName"] = data.CustomerFirstName,
            ["TenantName"] = data.TenantName,
            ["ReservationCode"] = data.ReservationCode,
            ["ApprovedAt"] = EmailDateFormatter.FormatDate(data.ApprovedAtUtc),
            ["VehicleDescription"] = $"{data.VehicleBrandName} {data.VehicleModelName} {data.VehicleYear}",
            ["RentalType"] = rentalType,
            ["RentalPeriod"] = FormatRentalPeriod(data.Quantity, data.RentalType),
            ["DeliveryDateTime"] = EmailDateFormatter.FormatDate(data.DeliveryDateTime),
            ["ExpectedReturnDateTime"] = EmailDateFormatter.FormatDate(data.ExpectedReturnDateTime),
            ["DeliveryLocation"] = data.DeliveryLocation,
            ["ReturnLocation"] = data.ReturnLocation,
            ["UnitRate"] = FormatMoney(data.UnitRate),
            ["RentalAmount"] = FormatMoney(data.RentalAmount),
            ["DeliveryFee"] = FormatMoney(data.DeliveryFee),
            ["ReturnFee"] = FormatMoney(data.ReturnFee),
            ["SecurityDepositDisplay"] = data.SecurityDepositRequired ? FormatMoney(data.SecurityDepositAmount) : "No requerido",
            ["DiscountAmount"] = data.DiscountAmount > 0 ? $"-{FormatMoney(data.DiscountAmount)}" : FormatMoney(0),
            ["TotalAmount"] = FormatMoney(data.TotalAmount),
            ["TenantPhone"] = data.TenantPhone.Value,
            ["TenantWhatsApp"] = data.TenantWhatsApp.Value,
            ["TenantWhatsAppUrl"] = BuildWhatsAppUrl(data.TenantWhatsApp.Value),
            ["TenantEmail"] = data.TenantEmail.Value
        };
    }

    private static string FormatMoney(decimal amount) => $"RD$ {amount.ToString("N2", DominicanCulture)}";

    private static string TranslateRentalType(RentalType rentalType) => rentalType switch
    {
        RentalType.Daily => "Diaria",
        RentalType.Weekly => "Semanal",
        RentalType.Monthly => "Mensual",
        _ => throw new InvalidOperationException("Reservation rental type is invalid.")
    };

    private static string FormatRentalPeriod(int quantity, RentalType rentalType)
    {
        string unit = rentalType switch
        {
            RentalType.Daily => quantity == 1 ? "día" : "días",
            RentalType.Weekly => quantity == 1 ? "semana" : "semanas",
            RentalType.Monthly => quantity == 1 ? "mes" : "meses",
            _ => throw new InvalidOperationException("Reservation rental type is invalid.")
        };

        return $"{quantity} {unit}";
    }

    private static string BuildWhatsAppUrl(string phoneNumber)
    {
        string digits = new(phoneNumber.Where(char.IsDigit).ToArray());
        return string.IsNullOrEmpty(digits) ? string.Empty : $"https://wa.me/{digits}";
    }
}
